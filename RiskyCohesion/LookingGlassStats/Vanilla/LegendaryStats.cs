using System.Collections.Generic;
using EntityStates.Headstompers;
using LookingGlass.ItemStatsNameSpace;
using RiskyCohesion.Sources;
using RiskyMod.Items;
using RiskyMod.Items.DLC1.Legendary;
using RiskyMod.Items.DLC2;
using RiskyMod.Items.DLC3.Legendary;
using RiskyMod.Items.Legendary;
using RoR2;
using UnityEngine;
using Style = LookingGlass.ItemStatsNameSpace.ItemStatsDef.ValueType;
using Unit = LookingGlass.ItemStatsNameSpace.ItemStatsDef.MeasurementUnits;

namespace RiskyCohesion.LookingGlassStats.Vanilla;

internal static class LegendaryStats
{
    private static readonly float FrostRelicMaxRadius = 18f;
    private static readonly float FrostRelicTicksPerSecond = 4f;
    private static readonly float FrostRelicProcCoefficient = 0.2f;
    private static readonly float GhostRechargeTime = 20f;
    private static readonly float NetworkedSufferingDamageShared = 0.25f;

    public static void Register(StatsGroup stats)
    {
        if (RiskyModRemovesProcChain(Tesla.enabled))
            stats.AmendItem(() => RoR2Content.Items.ShockNearby, WithoutProcCoefficient);

        if (RiskyModChanges(FrostRelic.enabled))
            stats.Item(() => RoR2Content.Items.Icicle, FrostRelicStats);

        if (RiskyModRemovesProcChain(CeremonialDagger.enabled))
            stats.AmendItem(() => RoR2Content.Items.Dagger, WithoutProcCoefficient);

        if (RiskyModChanges(HeadHunter.enabled))
            stats.Item(() => RoR2Content.Items.HeadHunter, WakeOfVulturesStats);

        if (RiskyModChanges(Headstompers.enabled))
            stats.Item(() => RoR2Content.Items.FallBoots, HeadstompersStats);

        if (RiskyModChanges(Behemoth.enabled))
            stats.Item(() => RoR2Content.Items.Behemoth, BrilliantBehemothStats);

        if (RiskyModChanges(HappiestMask.enabled))
            stats.Item(() => RoR2Content.Items.GhostOnKill, HappiestMaskStats);

        if (RiskyModChanges(Clover.enabled))
            stats.Item(() => RoR2Content.Items.Clover, CloverStats);

        if (RiskyModChanges(Aegis.enabled))
            stats.Item(() => RoR2Content.Items.BarrierOnOverHeal, AegisStats);

        if (RiskyModChanges(Scorpion.enabled))
            stats.Item(() => DLC1Content.Items.PermanentDebuffOnHit, SymbioticScorpionStats);

        if (RiskyModChanges(LaserScope.enabled))
            stats.Item(() => DLC1Content.Items.CritDamage, LaserScopeStats);

        if (RiskyModRemovesProcChain(Overspill.enabled))
            stats.AmendItem(() => DLC2Content.Items.MeteorAttackOnHighDamage, WithoutProcCoefficient);

        if (RiskyModChanges(GrowthNectar.enabled))
            stats.Item(() => DLC2Content.Items.BoostAllStats, GrowthNectarStats);

        if (RiskyModChanges(NetworkedSuffering.enabled))
            stats.AmendItem(() => DLC3Content.Items.SharedSuffering, NetworkedSufferingStats);
    }

    private static bool RiskyModChanges(bool itemEnabled) => VanillaStats.RiskyModChanges(ItemsCore.legendaryEnabled, itemEnabled);

    private static bool RiskyModRemovesProcChain(bool itemEnabled) => RiskyModChanges(itemEnabled) && RiskyModSource.DisablesProcChains;

    private static ItemStatsDef WithoutProcCoefficient(ItemStatsDef original) => original.WithProcCoefficient(() => 0f);

    private static ItemStatsDef FrostRelicStats() => new StatSheet()
        .Line("Max Radius: ", Style.Damage, Unit.Meters)
        .Line("Damage Per Second: ", Style.Damage, Unit.Percentage)
        .ProcCoefficient()
        .PerStack(stack =>
        [
            FrostRelicMaxRadius,
            FrostRelicTicksPerSecond * (2f + stack),
            VanillaStats.ProcCoefficient(FrostRelicProcCoefficient),
        ]);

    private static ItemStatsDef WakeOfVulturesStats() => new StatSheet()
        .Line("Buff Duration: ", Style.Damage, Unit.Seconds)
        .Line("Bonus Damage to Elites: ", Style.Damage, Unit.Percentage)
        .PerStack(stack => [5f + 5f * stack, 0.3f]);

    private static ItemStatsDef HeadstompersStats() => new StatSheet()
        .Line("Recharge Time: ", Style.Damage, Unit.Seconds)
        .Line("Min Damage: ", Style.Damage, Unit.Percentage)
        .Line("Max Damage: ", Style.Damage, Unit.Percentage)
        .ProcCoefficient()
        .PerStack(stack =>
        [
            StompRechargeTime(stack),
            HeadstompersFall.minimumDamageCoefficient * StompDamageMultiplier(stack),
            HeadstompersFall.maximumDamageCoefficient * StompDamageMultiplier(stack),
            1f,
        ]);

    private static float StompRechargeTime(int stack) => Headstompers.useBuff
        ? HeadstompersCooldown.baseDuration
        : HeadstompersCooldown.baseDuration / stack;

    private static float StompDamageMultiplier(int stack) => StatMath.Linear(1f, 0.6f, stack);

    private static ItemStatsDef BrilliantBehemothStats() => new StatSheet()
        .Line("Explosion Radius: ", Style.Damage, Unit.Meters)
        .ProcCoefficient()
        .PerStack(stack => [BehemothRadius(stack), 0f]);

    private static float BehemothRadius(int stack)
    {
        var radius = 0f;
        for (var i = 0; i < stack; i++)
            radius += Mathf.Max(1.25f, 4f * Mathf.Pow(0.625f, i));

        return radius;
    }

    private static ItemStatsDef HappiestMaskStats()
    {
        var sheet = new StatSheet()
            .Line("Recharge Time: ", Style.Utility, Unit.Seconds)
            .Line("Ghost Damage: ", Style.Damage, Unit.Percentage)
            .Line("Ghost Duration: ", Style.Damage, Unit.Seconds);

        if (HappiestMask.noGhostLimit)
            return sheet.PerStack(GhostStats);

        return sheet
            .Line("Max Ghosts: ", Style.Utility, Unit.Number)
            .PerStack(stack => [.. GhostStats(stack), MaxGhosts(stack)]);
    }

    private static List<float> GhostStats(int stack) => [GhostRechargeTime, StatMath.BoostMultiplier(32 + 18 * stack), 30f * stack];

    private static float MaxGhosts(int stack) => HappiestMask.scaleCount
        ? 2 + stack
        : 3;

    private static ItemStatsDef CloverStats() => new StatSheet()
        .Line("Luck: ", Style.Utility, Unit.Number)
        .Line("Bonus Stats: ", Style.Utility, Unit.Percentage)
        .Line("Bonus Armor: ", Style.Armor, Unit.Number)
        .PerStack(stack =>
        [
            Mathf.Min(stack, Clover.luckCap),
            0.4f * LuckAboveCap(stack),
            40f * LuckAboveCap(stack),
        ]);

    private static float LuckAboveCap(int stack) => Mathf.Max(stack - Clover.luckCap, 0f);

    private static ItemStatsDef AegisStats() => new StatSheet()
        .Line("Healing Converted: ", Style.Healing, Unit.Percentage)
        .Line("Barrier Decay Reduction: ", Style.Healing, Unit.Percentage)
        .PerStack(stack => [0.5f * stack, 1f - 1f / (1f + stack)]);

    private static ItemStatsDef SymbioticScorpionStats() => new StatSheet()
        .Line("Armor Reduced On-Hit: ", Style.Damage, Unit.Number)
        .PerStack(stack => [stack]);

    private static ItemStatsDef LaserScopeStats() => new StatSheet()
        .Line("Bonus Crit Damage: ", Style.Damage, Unit.Percentage)
        .Line("Crit Chance: ", Style.Damage, Unit.Percentage)
        .WithLuck((luck, stack, _) => [stack, StatMath.WithLuck(0.05f, luck)]);

    private static ItemStatsDef GrowthNectarStats() => new StatSheet()
        .Line("Max Stacks: ", Style.Shrine, Unit.Number)
        .Line("Max Bonus Stats: ", Style.Shrine, Unit.Percentage)
        .PerStack(stack => [4 * stack, 4 * stack * 0.05f]);

    private static ItemStatsDef NetworkedSufferingStats(ItemStatsDef original) => original.WithValue("Networked Damage: ", () => NetworkedSufferingDamageShared);
}
