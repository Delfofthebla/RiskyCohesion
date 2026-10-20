using System;
using System.Collections.Generic;
using LookingGlass.ItemStatsNameSpace;
using RiskyCohesion.Sources;
using RiskyMod.Items;
using RiskyMod.Items.Common;
using RiskyMod.Items.DLC1.Common;
using RiskyMod.Items.DLC3.Common;
using RoR2;
using Scaling = LookingGlass.ItemStatsNameSpace.ItemStatsDef.ChanceScaling;
using Style = LookingGlass.ItemStatsNameSpace.ItemStatsDef.ValueType;
using Unit = LookingGlass.ItemStatsNameSpace.ItemStatsDef.MeasurementUnits;

namespace RiskyCohesion.LookingGlassStats.Vanilla;

internal static class CommonStats
{
    private static readonly float FireworkDamage = 3f;

    public static void Register(StatsGroup stats)
    {
        if (RiskyModChanges(BisonSteak.enabled))
            stats.Item(() => RoR2Content.Items.FlatHealth, BisonSteakStats);

        if (RiskyModChanges(CritGlasses.enabled))
            stats.Item(() => RoR2Content.Items.CritGlasses, LensMakersGlassesStats);

        if (RiskyModChanges(Fireworks.enabled))
            stats.Item(() => RoR2Content.Items.Firework, FireworksStats);

        if (RiskyModChanges(Crowbar.enabled))
            stats.Item(() => RoR2Content.Items.Crowbar, CrowbarStats);

        if (RiskyModChanges(Warbanner.enabled))
            stats.Item(() => RoR2Content.Items.WardOnLevel, WarbannerStats);

        if (RiskyModChanges(TougherTimes.enabled))
            stats.Item(() => RoR2Content.Items.Bear, TougherTimesStats);

        if (RiskyModChanges(StunGrenade.enabled))
            stats.Item(() => RoR2Content.Items.StunChanceOnHit, StunGrenadeStats);

        if (RiskyModChanges(DelicateWatch.enabled))
            stats.Item(() => DLC1Content.Items.FragileDamageBonus, DelicateWatchStats);

        if (RiskyModChanges(PowerElixir.enabled))
            stats.Item(() => DLC1Content.Items.HealingPotionConsumed, EmptyBottleStats);

        if (RiskyModChanges(EclipseLite.enabled))
            stats.Item(() => DLC3Content.Items.BarrierOnCooldown, EclipseLiteStats);
    }

    public static void RegisterGasoline(StatsGroup stats)
    {
        var fixedRadius = RiskyModSource.ChangesGasoline;
        var fasterBurn = RiskyTweaksSource.GasolineBurnsFaster;
        if (!fixedRadius && !fasterBurn)
            return;

        stats.Item(() => RoR2Content.Items.IgniteOnKill, () => GasolineStats(fixedRadius, fasterBurn));
    }

    private static bool RiskyModChanges(bool itemEnabled) => VanillaStats.RiskyModChanges(ItemsCore.commonEnabled, itemEnabled);

    private static ItemStatsDef BisonSteakStats() => new StatSheet()
        .Line("Flat Health: ", Style.Healing, Unit.FlatHealth)
        .Line("Health Increase as %: ", Style.Healing, Unit.PercentHealth)
        .PerBody(BisonSteakHealth);

    private static List<float> BisonSteakHealth(CharacterBody body, int stack)
    {
        if (!body)
            return null;

        var health = body.levelMaxHealth * stack;

        return [health, health / Holder.LevelHealth(body)];
    }

    private static ItemStatsDef LensMakersGlassesStats() => new StatSheet()
        .Line("Crit Chance: ", Style.Damage, Unit.Percentage)
        .WithLuck((luck, stack, _) => [StatMath.WithLuck(StatMath.Linear(0.1f, 0.07f, stack), luck)]);

    private static ItemStatsDef FireworksStats() => new StatSheet()
        .Line("Fireworks: ", Style.Damage, Unit.Number)
        .Line("Damage Per Firework: ", Style.Damage, Unit.Percentage)
        .ProcCoefficient()
        .PerStack(stack =>
        [
            Math.Min(FireworksLaunched(stack), Fireworks.maxRockets),
            FireworkDamage * Math.Max(1f, (float)FireworksLaunched(stack) / Fireworks.maxRockets),
            ItemValues.FireworkProcCoefficient,
        ]);

    private static int FireworksLaunched(int stack) => 4 + 4 * stack;

    private static ItemStatsDef CrowbarStats() => new StatSheet()
        .Line("Bonus Damage: ", Style.Damage, Unit.Percentage)
        .PerStack(stack => [Crowbar.damageCoefficient * stack]);

    private static ItemStatsDef WarbannerStats() => new StatSheet()
        .Line("Effect Radius: ", Style.Utility, Unit.Meters)
        .Line("Bonus Damage: ", Style.Damage, Unit.Percentage)
        .Line("Healing: ", Style.Healing, Unit.PercentHealing)
        .PerStack(stack => [StatMath.Linear(16f, 8f, stack), 0.1f, 0.01f]);

    private static ItemStatsDef GasolineStats(bool fixedRadius, bool fasterBurn) => new StatSheet()
        .Line("Total Damage: ", Style.Damage, Unit.Percentage)
        .Line("Burn Duration: ", Style.Damage, Unit.Seconds)
        .Line("Radius: ", Style.Utility, Unit.Meters)
        .ProcCoefficient()
        .PerStack(stack =>
        [
            StatMath.Linear(3f, 0.75f, stack),
            StatMath.Linear(3f, 1.5f, stack) / BurnSpeed(fasterBurn, stack),
            IgniteRadius(fixedRadius, stack),
            0f,
        ]);

    private static float BurnSpeed(bool fasterBurn, int stack) => fasterBurn
        ? StatMath.Linear(1f, 0.25f, stack)
        : 1f;

    private static float IgniteRadius(bool fixedRadius, int stack) => fixedRadius
        ? 16f
        : StatMath.Linear(12f, 4f, stack);

    private static ItemStatsDef TougherTimesStats() => new StatSheet()
        .Line("Block Chance: ", Style.Utility, Unit.Percentage)
        .PerStack(stack => [StatMath.Hyperbolic(0.1f, 0.1f, stack)]);

    private static ItemStatsDef StunGrenadeStats() => new StatSheet()
        .Line("Stun Chance: ", Style.Utility, Unit.Percentage)
        .Line("Stun Damage: ", Style.Damage, Unit.Percentage)
        .ProcCoefficient()
        .Chance(Scaling.Linear, (luck, stack, procCoefficient) =>
        [
            StatMath.WithLuck(SetStateOnHurt.stunChanceOnHitBaseChancePercent / 100f * stack * procCoefficient, luck),
            0.5f,
            0f,
        ]);

    private static ItemStatsDef DelicateWatchStats() => new StatSheet()
        .Line("Bonus Damage: ", Style.Damage, Unit.Percentage)
        .PerStack(stack => [0.15f * stack]);

    private static ItemStatsDef EmptyBottleStats() => new StatSheet()
        .Line("Barrier Gained: ", Style.Healing, Unit.PercentHealth)
        .PerStack(stack => [0.5f * stack]);

    private static ItemStatsDef EclipseLiteStats() => new StatSheet()
        .Line("Barrier per cooldown: ", Style.Healing, Unit.PercentHealth)
        .PerStack(stack => [StatMath.Linear(0.01f, 0.005f, stack)]);
}
