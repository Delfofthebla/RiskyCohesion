using LookingGlass.ItemStatsNameSpace;
using RiskyCohesion.Sources;
using RiskyMod.Items;
using RiskyMod.Items.Lunar;
using RoR2;
using UnityEngine;
using Style = LookingGlass.ItemStatsNameSpace.ItemStatsDef.ValueType;
using Unit = LookingGlass.ItemStatsNameSpace.ItemStatsDef.MeasurementUnits;

namespace RiskyCohesion.LookingGlassStats.Vanilla;

internal static class LunarStats
{
    private static readonly string RechargeTimeDescription = "Recharge Time: ";
    private static readonly float MinimumRechargeSpeed = 0.5f;

    public static void Register(StatsGroup stats)
    {
        if (VanillaStats.RiskyModChanges(ItemsCore.lunarEnabled, ShapedGlass.enabled))
            stats.Item(() => RoR2Content.Items.LunarDagger, ShapedGlassStats);
    }

    public static void RegisterVisionsOfHeresy(StatsGroup stats)
    {
        if (RiskyTweaksSource.VisionsScaleWithAttackSpeed)
            stats.AmendItem(() => RoR2Content.Items.LunarPrimaryReplacement, VisionsOfHeresyStats);
    }

    private static ItemStatsDef ShapedGlassStats() => new StatSheet()
        .Line("Damage Multiplier: ", Style.Damage, Unit.Percentage)
        .Line("Health Multiplier: ", Style.Death, Unit.Percentage)
        .PerStack(stack => [1f + stack, 1f / (1f + stack)]);

    private static ItemStatsDef VisionsOfHeresyStats(ItemStatsDef original) => original.WithValueFromBody(RechargeTimeDescription, RechargeScaledByAttackSpeed);

    private static float RechargeScaledByAttackSpeed(float recharge, CharacterBody body)
    {
        var restockSpeed = 1f + body.attackSpeed - body.baseAttackSpeed;
        if (restockSpeed <= 0f)
            return float.PositiveInfinity;

        // The game also speeds the recharge by attack speed once RiskyTweaks sets attackSpeedBuffsRestockSpeed, on top of RiskyTweaks' shorter interval.
        return recharge / Mathf.Max(MinimumRechargeSpeed, restockSpeed) / restockSpeed;
    }
}
