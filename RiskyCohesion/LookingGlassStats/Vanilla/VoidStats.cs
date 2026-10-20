using System;
using LookingGlass.ItemStatsNameSpace;
using RiskyCohesion.Sources;
using RiskyMod.Items;
using RiskyMod.Items.DLC1.Void;
using RoR2;
using UnityEngine;
using Scaling = LookingGlass.ItemStatsNameSpace.ItemStatsDef.ChanceScaling;
using Style = LookingGlass.ItemStatsNameSpace.ItemStatsDef.ValueType;
using Unit = LookingGlass.ItemStatsNameSpace.ItemStatsDef.MeasurementUnits;

namespace RiskyCohesion.LookingGlassStats.Vanilla;

internal static class VoidStats
{
    private static readonly float VoidsentFlameRadius = 12f;
    private static readonly float ZoeaBonusDamagePerExtraStack = 0.3f;

    public static void Register(StatsGroup stats)
    {
        if (RiskyModChanges(Dungus.enabled))
            stats.Item(() => DLC1Content.Items.MushroomVoid, WeepingFungusStats);

        if (RiskyModChanges(VoidLens.enabled))
            stats.Item(() => DLC1Content.Items.CritGlassesVoid, LostSeersLensesStats);

        if (RiskyModChanges(PlasmaShrimp.enabled))
            stats.Item(() => DLC1Content.Items.MissileVoid, PlasmaShrimpStats);

        if (RiskyModChanges(VoidWisp.enabled))
            stats.Item(() => DLC1Content.Items.ExplodeOnDeathVoid, VoidsentFlameStats);

        if (RiskyModChanges(Polylute.enabled))
            stats.Item(() => DLC1Content.Items.ChainLightningVoid, PolyluteStats);

        if (RiskyModChanges(VoidRing.enabled))
            stats.Item(() => DLC1Content.Items.ElementalRingVoid, SingularityBandStats);

        if (RiskyModChanges(Zoea.enabled))
            stats.Item(() => DLC1Content.Items.VoidMegaCrabItem, NewlyHatchedZoeaStats);
    }

    public static void RegisterSaferSpaces(StatsGroup stats)
    {
        var longerRecharge = RiskyModSource.ChangesSaferSpaces;
        var correctedStacking = RiskyFixesSource.CorrectsSaferSpacesCooldown;
        if (!longerRecharge && !correctedStacking)
            return;

        var recharge = longerRecharge ? ItemValues.SaferSpacesRiskyModRecharge : ItemValues.SaferSpacesRecharge;
        var uncountedStacks = correctedStacking ? 1 : 0;

        stats.Item(() => DLC1Content.Items.BearVoid, () => SaferSpacesStats(recharge, uncountedStacks));
    }

    private static bool RiskyModChanges(bool itemEnabled) => VanillaStats.RiskyModChanges(ItemsCore.voidEnabled, itemEnabled);

    private static ItemStatsDef WeepingFungusStats() => new StatSheet()
        .Line("Sprint Healing: ", Style.Healing, Unit.PercentHealing)
        .PerStack(stack => [(Dungus.baseHealPercent + Dungus.stackHealPercent * stack) / MushroomVoidBehavior.healPeriodSeconds]);

    private static ItemStatsDef LostSeersLensesStats() => new StatSheet()
        .Line("Crit Chance: ", Style.Damage, Unit.Percentage)
        .WithLuck((luck, stack, _) => [StatMath.WithLuck(0.1f * stack, luck)]);

    private static ItemStatsDef SaferSpacesStats(float recharge, int uncountedStacks) => new StatSheet()
        .Line("Recharge Time: ", Style.Utility, Unit.Seconds)
        .PerStack(stack => [recharge * Mathf.Pow(ItemValues.SaferSpacesRechargeKeptPerStack, stack - uncountedStacks)]);

    private static ItemStatsDef PlasmaShrimpStats() => new StatSheet()
        .Line("Proc Chance: ", Style.Damage, Unit.Percentage)
        .Line("Total Damage: ", Style.Damage, Unit.Percentage)
        .ProcCoefficient()
        .Chance(Scaling.DoesNotScale, (luck, stack, procCoefficient) =>
        [
            StatMath.WithLuck(Mathf.Min(procCoefficient, 1f), luck),
            StatMath.Linear(0.4f, 0.28f, stack),
            VanillaStats.ProcCoefficient(0.2f),
        ]);

    private static ItemStatsDef VoidsentFlameStats() => new StatSheet()
        .Line("Radius: ", Style.Damage, Unit.Meters)
        .Line("Pillar Damage: ", Style.Damage, Unit.Percentage)
        .ProcCoefficient()
        .PerStack(stack => [VoidsentFlameRadius, StatMath.Linear(2.6f, 1.56f, stack), VanillaStats.ProcCoefficient(1f)]);

    private static ItemStatsDef PolyluteStats() => new StatSheet()
        .Line("Proc Chance: ", Style.Damage, Unit.Percentage)
        .Line("Total Hits: ", Style.Damage, Unit.Number)
        .ProcCoefficient()
        .Chance(Scaling.DoesNotScale, (luck, stack, procCoefficient) =>
        [
            StatMath.WithLuck(0.25f * procCoefficient, luck),
            StatMath.Linear(3f, 2f, stack),
            VanillaStats.ProcCoefficient(0.2f),
        ]);

    private static ItemStatsDef SingularityBandStats() => new StatSheet()
        .Line("Total Damage: ", Style.Damage, Unit.Percentage)
        .ProcCoefficient()
        .PerStack(stack => [StatMath.Linear(2f, 1.2f, stack), VanillaStats.ProcCoefficient(1f)]);

    private static ItemStatsDef NewlyHatchedZoeaStats() => new StatSheet()
        .Line("Max Allies: ", Style.Utility, Unit.Number)
        .Line("Recharge Time: ", Style.Utility, Unit.Seconds)
        .Line("Ally Bonus Damage: ", Style.Damage, Unit.Percentage)
        .PerStack(stack =>
        [
            Math.Min(stack, Zoea.maxAllyCount),
            60f / stack,
            ZoeaBonusDamagePerExtraStack * Math.Max(0, stack - Zoea.maxAllyCount),
        ]);
}
