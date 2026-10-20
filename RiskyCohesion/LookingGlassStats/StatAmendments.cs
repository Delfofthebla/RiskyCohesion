using System;
using System.Collections.Generic;
using LookingGlass.ItemStatsNameSpace;
using RoR2;

namespace RiskyCohesion.LookingGlassStats;

internal static class StatAmendments
{
    public static ItemStatsDef WithProcCoefficient(this ItemStatsDef original, Func<float> value) => Existing(original).descriptions.Contains(StatSheet.ProcCoefficientDescription)
        ? original.WithValue(StatSheet.ProcCoefficientDescription, value)
        : original.WithLine(StatSheet.ProcCoefficientDescription, ItemStatsDef.ValueType.Damage, ItemStatsDef.MeasurementUnits.ProcCoeff, value);

    public static ItemStatsDef WithLine(this ItemStatsDef original, string description, ItemStatsDef.ValueType style, ItemStatsDef.MeasurementUnits unit, Func<float> value)
    {
        var amended = Copy(Existing(original), (values, _) => [.. values, value()]);

        amended.descriptions.Add(description);
        amended.valueTypes.Add(style);
        amended.measurementUnits.Add(unit);

        return amended;
    }

    public static ItemStatsDef WithValue(this ItemStatsDef original, string description, Func<float> value) => original.WithValue(description, _ => value());

    public static ItemStatsDef WithValue(this ItemStatsDef original, string description, Func<int, float> valueForStack)
    {
        var line = LineOf(original, description);

        return Copy(original, (values, stack) => Replace(values, line, valueForStack(stack)));
    }

    public static ItemStatsDef WithValueFromBody(this ItemStatsDef original, string description, Func<float, CharacterBody, float> change)
    {
        var line = LineOf(original, description);
        var valuesPerStack = original.calculateValuesFlat ?? throw new InvalidOperationException($"LookingGlass' stats aren't computed per stack, so \"{description}\" can't follow the holder.");

        var amended = CopyLines(original);
        amended.calculateValuesBody = (body, stack) => ValuesForBody(valuesPerStack, line, change, body, stack);

        return amended;
    }

    public static ItemStatsDef WithUnit(this ItemStatsDef original, string description, ItemStatsDef.MeasurementUnits unit)
    {
        var line = LineOf(original, description);

        var amended = Copy(original, (values, _) => values);
        amended.measurementUnits[line] = unit;

        return amended;
    }

    private static ItemStatsDef Existing(ItemStatsDef original) => original ?? throw new InvalidOperationException("LookingGlass has no stats for this item to amend.");

    private static int LineOf(ItemStatsDef original, string description)
    {
        var line = Existing(original).descriptions.IndexOf(description);
        if (line < 0)
            throw new InvalidOperationException($"LookingGlass' stats have no \"{description}\" line to amend.");

        return line;
    }

    private static List<float> Replace(List<float> values, int line, float value)
    {
        List<float> replaced = [.. values];
        replaced[line] = value;

        return replaced;
    }

    private static List<float> ValuesForBody(Func<int, List<float>> valuesPerStack, int line, Func<float, CharacterBody, float> change, CharacterBody body, int stack)
    {
        var values = valuesPerStack(stack);
        if (values == null || !body)
            return null;

        return Replace(values, line, change(values[line], body));
    }

    private static ItemStatsDef CopyLines(ItemStatsDef original) => new()
    {
        descriptions = [.. original.descriptions],
        valueTypes = [.. original.valueTypes],
        measurementUnits = [.. original.measurementUnits],
        isScrap = original.isScrap,
        hasChance = original.hasChance,
        chanceScaling = original.chanceScaling,
    };

    private static ItemStatsDef Copy(ItemStatsDef original, Func<List<float>, int, List<float>> change)
    {
        var copy = CopyLines(original);

        if (original.calculateValuesFlat != null)
            copy.calculateValuesFlat = stack => ChangeIfComputed(change, original.calculateValuesFlat(stack), stack);

        if (original.calculateValuesNew != null)
            copy.calculateValuesNew = (luck, stack, procCoefficient) => ChangeIfComputed(change, original.calculateValuesNew(luck, stack, procCoefficient), stack);

        if (original.calculateValues != null)
            copy.calculateValues = (master, stack) => ChangeIfComputed(change, original.calculateValues(master, stack), stack);

        if (original.calculateValuesBody != null)
            copy.calculateValuesBody = (body, stack) => ChangeIfComputed(change, original.calculateValuesBody(body, stack), stack);

        return copy;
    }

    private static List<float> ChangeIfComputed(Func<List<float>, int, List<float>> change, List<float> values, int stack) => values == null
        ? null
        : change(values, stack);
}
