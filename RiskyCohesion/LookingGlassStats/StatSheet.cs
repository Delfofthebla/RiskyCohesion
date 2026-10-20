using System;
using System.Collections.Generic;
using LookingGlass.ItemStatsNameSpace;
using RoR2;

namespace RiskyCohesion.LookingGlassStats;

internal sealed class StatSheet
{
    public static readonly string ProcCoefficientDescription = "Proc Coefficient: ";

    private readonly ItemStatsDef _stats = new();

    public StatSheet ProcCoefficient() => Line(ProcCoefficientDescription, ItemStatsDef.ValueType.Damage, ItemStatsDef.MeasurementUnits.ProcCoeff);

    public StatSheet Line(string description, ItemStatsDef.ValueType style, ItemStatsDef.MeasurementUnits unit)
    {
        _stats.descriptions.Add(description);
        _stats.valueTypes.Add(style);
        _stats.measurementUnits.Add(unit);

        return this;
    }

    public ItemStatsDef PerStack(Func<int, List<float>> values)
    {
        _stats.calculateValuesFlat = values;
        return _stats;
    }

    public ItemStatsDef PerMaster(Func<CharacterMaster, int, List<float>> values)
    {
        _stats.calculateValues = values;
        return _stats;
    }

    public ItemStatsDef PerBody(Func<CharacterBody, int, List<float>> values)
    {
        _stats.calculateValuesBody = values;
        return _stats;
    }

    public ItemStatsDef WithLuck(Func<float, int, float, List<float>> values)
    {
        _stats.calculateValuesNew = values;
        return _stats;
    }

    public ItemStatsDef Chance(ItemStatsDef.ChanceScaling scaling, Func<float, int, float, List<float>> values)
    {
        _stats.hasChance = true;
        _stats.chanceScaling = scaling;

        return WithLuck(values);
    }
}
