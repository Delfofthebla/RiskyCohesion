using System.Collections.Generic;
using LookingGlass.ItemStatsNameSpace;
using RiskyMod.Items;
using RiskyMod.Items.DLC3.Food;
using RoR2;
using Style = LookingGlass.ItemStatsNameSpace.ItemStatsDef.ValueType;
using Unit = LookingGlass.ItemStatsNameSpace.ItemStatsDef.MeasurementUnits;

namespace RiskyCohesion.LookingGlassStats.Vanilla;

internal static class FoodStats
{
    public static void Register(StatsGroup stats)
    {
        if (RiskyModChanges(QuickFix.enabled))
            stats.Item(() => DLC3Content.Items.BonusHealthBoost, QuickFixStats);

        if (RiskyModChanges(SearedSteak.enabled))
            stats.Item(() => DLC3Content.Items.CookedSteak, SearedSteakStats);
    }

    private static bool RiskyModChanges(bool itemEnabled) => VanillaStats.RiskyModChanges(ItemsCore.foodEnabled, itemEnabled);

    private static ItemStatsDef QuickFixStats() => new StatSheet()
        .Line("Bonus Non-Level Health: ", Style.Healing, Unit.Percentage)
        .Line("Bonus Non-Level Regen: ", Style.Healing, Unit.Percentage)
        .PerStack(stack => [StatMath.Linear(0.5f, 0.25f, stack), StatMath.Linear(0.5f, 0.15f, stack)]);

    private static ItemStatsDef SearedSteakStats() => new StatSheet()
        .Line("Flat Health: ", Style.Healing, Unit.FlatHealth)
        .Line("Bonus Health: ", Style.Healing, Unit.Percentage)
        .PerBody(SearedSteakHealth);

    private static List<float> SearedSteakHealth(CharacterBody body, int stack)
    {
        if (!body)
            return null;

        return [body.levelMaxHealth * 1.5f * stack, 0.05f * stack];
    }
}
