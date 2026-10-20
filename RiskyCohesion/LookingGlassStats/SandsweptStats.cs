using System;
using BepInEx.Configuration;
using LookingGlass.ItemStatsNameSpace;
using RoR2;
using Sandswept.Items;
using Sandswept.Items.Greens;
using Sandswept.Items.Reds;
using Sandswept.Items.VoidGreens;
using Sandswept.Items.Whites;
using Style = LookingGlass.ItemStatsNameSpace.ItemStatsDef.ValueType;
using Unit = LookingGlass.ItemStatsNameSpace.ItemStatsDef.MeasurementUnits;

namespace RiskyCohesion.LookingGlassStats;

internal static class SandsweptStats
{
    public static void Install(ConfigFile config)
    {
        var description = new Description("Adds LookingGlass stats for Sacrificial Band, and proc coefficients to Sandswept's own stats.");
        var stats = StatsGroup.Bind(config, ContentMod.Sandswept.Name, description);

        stats.Item(ItemDefOf<SacrificialBand>, SacrificialBandStats);

        ShowProcCoefficient<AmberKnife>(stats, () => AmberKnife.projectileController.procCoefficient);
        ShowProcCoefficient<NuclearSalvo>(stats, () => NuclearSalvo.missileProcCoefficient);
        ShowProcCoefficient<SunFragment>(stats, () => SunFragment.explosionProcCoefficient);
        ShowProcCoefficient<Millenium>(stats, () => Millenium.explosionProcCoefficient);
        ShowProcCoefficient<CeremonialJar>(stats, () => CeremonialJar.procCoefficient);
    }

    private static ItemDef ItemDefOf<T>() where T : ItemBase<T> => ItemBase<T>.instance?.ItemDef;

    private static void ShowProcCoefficient<T>(StatsGroup stats, Func<float> coefficient) where T : ItemBase<T> => stats.AmendItem(ItemDefOf<T>, original => original.WithProcCoefficient(coefficient));

    private static ItemStatsDef SacrificialBandStats() => new StatSheet()
        .Line("Damage Threshold: ", Style.Damage, Unit.Percentage)
        .Line($"Bleeds per {Description.Format(SacrificialBand.damageScalar * 100f)}% Damage: ", Style.Damage, Unit.Number)
        .Line("Recharge Time: ", Style.Utility, Unit.Seconds)
        .PerStack(stack =>
        [
            SacrificialBand.percentDamageThreshold,
            StatMath.Linear(SacrificialBand.baseBleedCount, SacrificialBand.stackBleedCount, stack),
            SacrificialBand.cooldown,
        ]);
}
