using System.Collections.Generic;
using LookingGlass.ItemStatsNameSpace;
using RiskyCohesion.Sources;
using RiskyMod;
using RiskyMod.Items;
using RiskyMod.Items.Boss;
using RiskyMod.Items.DLC1.Boss;
using RiskyMod.Items.DLC3.Boss;
using RoR2;
using UnityEngine;
using Scaling = LookingGlass.ItemStatsNameSpace.ItemStatsDef.ChanceScaling;
using Style = LookingGlass.ItemStatsNameSpace.ItemStatsDef.ValueType;
using Unit = LookingGlass.ItemStatsNameSpace.ItemStatsDef.MeasurementUnits;

namespace RiskyCohesion.LookingGlassStats.Vanilla;

internal static class BossStats
{
    private static readonly float MeatballProcCoefficient = 0.7f;
    private static readonly float LightningStrikeProcCoefficient = 1f;
    private static readonly float SpleenExplosionProcCoefficient = 1f;
    private static readonly float DiscipleProcCoefficient = 0.5f;
    private static readonly float ConstructLifetime = 40f;
    private static readonly int HalcyonSeedHealthBoosts = 10;

    public static void Register(StatsGroup stats)
    {
        if (RiskyModChanges(QueensGland.enabled && !SoftDependencies.QueensGlandBuffLoaded))
            stats.Item(() => RoR2Content.Items.BeetleGland, QueensGlandStats);

        if (RiskyModChanges(MoltenPerf.enabled))
            stats.Item(() => RoR2Content.Items.FireballsOnHit, MoltenPerforatorStats);

        if (RiskyModChanges(ChargedPerf.enabled))
            stats.Item(() => RoR2Content.Items.LightningStrikeOnHit, ChargedPerforatorStats);

        if (RiskyModChanges(Shatterspleen.enabled))
            stats.Item(() => RoR2Content.Items.BleedOnHitAndExplode, ShatterspleenStats);

        if (RiskyModChanges(Knurl.enabled))
            stats.Item(() => RoR2Content.Items.Knurl, TitanicKnurlStats);

        if (RiskyModChanges(Disciple.enabled) && RiskyModSource.DisablesProcChains)
            stats.AmendItem(() => RoR2Content.Items.SprintWisp, LittleDiscipleStats);

        if (RiskyModChanges(DefenseNucleus.enabled))
            stats.Item(() => DLC1Content.Items.MinorConstructOnKill, DefenseNucleusStats);

        if (RiskyModChanges(HalcyonSeed.enabled))
            stats.Item(() => RoR2Content.Items.TitanGoldDuringTP, HalcyonSeedStats);

        if (RiskyModChanges(PrisonMatrix.enabled))
            stats.Item(() => DLC3Content.Items.PowerCube, PrisonMatrixStats);
    }

    private static bool RiskyModChanges(bool itemEnabled) => VanillaStats.RiskyModChanges(ItemsCore.bossEnabled, itemEnabled);

    private static ItemStatsDef QueensGlandStats() => new StatSheet()
        .Line("Beetle Guards: ", Style.Utility, Unit.Number)
        .Line("Guard Damage: ", Style.Damage, Unit.Percentage)
        .Line("Guard Health: ", Style.Healing, Unit.Percentage)
        .PerStack(stack =>
        [
            BeetleGuardCount(),
            StatMath.BoostMultiplier(StatMath.Linear(10f, 10f, stack)),
            StatMath.BoostMultiplier(StatMath.Linear(20f, 30f, stack)),
        ]);

    private static float BeetleGuardCount() => SwarmsEnabled()
        ? 2f
        : 1f;

    private static bool SwarmsEnabled() => RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.swarmsArtifactDef);

    private static ItemStatsDef MoltenPerforatorStats() => new StatSheet()
        .Line("Proc Chance: ", Style.Damage, Unit.Percentage)
        .Line("Total Damage: ", Style.Damage, Unit.Percentage)
        .ProcCoefficient()
        .Chance(Scaling.DoesNotScale, (luck, stack, procCoefficient) =>
        [
            StatMath.WithLuck(0.1f * procCoefficient, luck),
            StatMath.Linear(MoltenPerf.initialDamageCoefficient, MoltenPerf.stackDamageCoefficient, stack),
            VanillaStats.ProcCoefficient(MeatballProcCoefficient),
        ]);

    private static ItemStatsDef ChargedPerforatorStats() => new StatSheet()
        .Line("Proc Chance: ", Style.Damage, Unit.Percentage)
        .Line("Total Damage: ", Style.Damage, Unit.Percentage)
        .ProcCoefficient()
        .Chance(Scaling.DoesNotScale, (luck, stack, procCoefficient) =>
        [
            StatMath.WithLuck(0.1f * procCoefficient, luck),
            StatMath.Linear(ChargedPerf.initialDamageCoefficient, ChargedPerf.stackDamageCoefficient, stack),
            VanillaStats.ProcCoefficient(LightningStrikeProcCoefficient),
        ]);

    private static ItemStatsDef ShatterspleenStats() => new StatSheet()
        .Line("Bleed Chance: ", Style.Damage, Unit.Percentage)
        .Line("Explosion Base Damage: ", Style.Damage, Unit.Percentage)
        .Line("Explosion Scaling: ", Style.Damage, Unit.PercentHealth)
        .Line("Explosion Radius: ", Style.Damage, Unit.Meters)
        .ProcCoefficient()
        .Chance(Scaling.DoesNotScale, (luck, stack, procCoefficient) =>
        [
            StatMath.WithLuck(0.1f * procCoefficient, luck),
            StatMath.Linear(4f, 3.2f, stack),
            StatMath.Linear(0.1f, 0.08f, stack),
            16f,
            VanillaStats.ProcCoefficient(SpleenExplosionProcCoefficient),
        ]);

    private static ItemStatsDef TitanicKnurlStats() => new StatSheet()
        .Line("Bonus Health: ", Style.Healing, Unit.PercentHealth)
        .Line("Base Regen: ", Style.Healing, Unit.FlatHealing)
        .Line("Bonus Armor: ", Style.Armor, Unit.Number)
        .PerMaster((master, stack) => [0.08f * stack, 1.6f * stack * Holder.RegenMultiplier(master), 10f * stack]);

    private static ItemStatsDef LittleDiscipleStats(ItemStatsDef original) => original.WithProcCoefficient(() => DiscipleProcCoefficient);

    private static ItemStatsDef DefenseNucleusStats() => new StatSheet()
        .Line("Max Constructs: ", Style.Utility, Unit.Number)
        .Line("Construct Damage: ", Style.Damage, Unit.Percentage)
        .Line("Construct Health: ", Style.Healing, Unit.Percentage)
        .Line("Construct Duration: ", Style.Utility, Unit.Seconds)
        .PerStack(DefenseNucleusValues);

    private static List<float> DefenseNucleusValues(int stack)
    {
        var maxConstructs = DefenseNucleus.removeAllyScaling
            ? 4f
            : 4f * stack;

        var constructBoosts = DefenseNucleus.removeAllyScaling
            ? StatMath.Linear(30f, 20f, stack)
            : 30f;

        var constructStats = StatMath.BoostMultiplier(constructBoosts);

        return [maxConstructs, constructStats, constructStats, ConstructLifetime];
    }

    private static ItemStatsDef HalcyonSeedStats() => new StatSheet()
        .Line("Aurelionite Damage: ", Style.Damage, Unit.Percentage)
        .Line("Aurelionite Health: ", Style.Healing, Unit.Percentage)
        .PerMaster(HalcyonSeedValues);

    private static List<float> HalcyonSeedValues(CharacterMaster holder, int stack)
    {
        var seeds = Holder.LivingItemCount(holder, RoR2Content.Items.TitanGoldDuringTP, stack);

        return
        [
            StatMath.BoostMultiplier(StatMath.BoostItemsFor(Mathf.Sqrt(seeds))),
            StatMath.BoostMultiplier(StatMath.BoostItemsFor(seeds) + HalcyonSeedHealthBoosts),
        ];
    }

    private static ItemStatsDef PrisonMatrixStats() => new StatSheet()
        .Line("Bonus Armor: ", Style.Armor, Unit.Number)
        .Line("Armor Increase: ", Style.Healing, Unit.Percentage)
        .PerStack(_ => [10f, 0.5f]);
}
