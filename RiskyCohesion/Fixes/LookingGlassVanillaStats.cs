using System.Collections.Generic;
using BepInEx.Configuration;
using EntityStates.Headstompers;
using EntityStates.VagrantNovaItem;
using LookingGlass.ItemStatsNameSpace;
using RiskyCohesion.LookingGlassStats;
using RoR2;
using UnityEngine;
using Scaling = LookingGlass.ItemStatsNameSpace.ItemStatsDef.ChanceScaling;
using Style = LookingGlass.ItemStatsNameSpace.ItemStatsDef.ValueType;
using Unit = LookingGlass.ItemStatsNameSpace.ItemStatsDef.MeasurementUnits;

namespace RiskyCohesion.Fixes;

internal static class LookingGlassVanillaStats
{
    private static readonly float TentabaublePercentChancePerStack = 5f;
    private static readonly float MeatHookChancePerStack = 0.2f;
    private static readonly float MeatHookDamage = 1f;
    private static readonly int MeatHookTargetsPerStack = 5;
    private static readonly float MeatHookRadius = 30f;
    private static readonly float MeatHookProcCoefficient = 0.33f;
    private static readonly float InfusionCapPerStack = 100f;
    private static readonly float BisonSteakHealthPerStack = 25f;

    public static void Install(ConfigFile config)
    {
        var description = new Description("Corrects mistakes in LookingGlass' own stats for vanilla items.");
        var stats = StatsGroup.CorrectingLookingGlass(config, "Vanilla Stat Fixes", description);

        stats.AmendItem(() => DLC1Content.Items.LunarSun, original => original.WithValue("Orb Cooldown: ", EgocentrismCooldown));
        stats.AmendItem(() => RoR2Content.Items.NovaOnLowHealth, original => original.WithValue("Recharge Time: ", GenesisLoopRecharge));
        stats.AmendItem(() => RoR2Content.Items.FallBoots, original => original.WithValue("Recharge Time: ", HeadstompersRecharge));
        stats.AmendItem(() => RoR2Content.Items.ChainLightning, original => original.WithValue("Chain Distance: ", UkuleleRange));
        stats.AmendItem(() => RoR2Content.Items.Firework, original => original.WithProcCoefficient(FireworksProcCoefficient));
        stats.AmendItem(() => RoR2Content.Items.IceRing, original => original.WithProcCoefficient(RunaldsBandProcCoefficient));
        stats.AmendItem(() => RoR2Content.Items.IgniteOnKill, original => original.WithUnit("Burn Duration: ", Unit.Seconds));
        stats.AmendItem(() => RoR2Content.Items.Icicle, original => original.WithUnit("Max Stacks: ", Unit.Number));

        stats.Item(() => DLC1Content.Items.SlowOnHitVoid, TentabaubleStats);
        stats.Item(() => RoR2Content.Items.StunChanceOnHit, StunGrenadeStats);
        stats.Item(() => RoR2Content.Items.BounceNearby, SentientMeatHookStats);
        stats.Item(() => RoR2Content.Items.TitanGoldDuringTP, HalcyonSeedStats);
        stats.Item(() => RoR2Content.Items.TPHealingNova, LeptonDaisyStats);
        stats.Item(() => RoR2Content.Items.Infusion, InfusionStats);
        stats.Item(() => RoR2Content.Items.FlatHealth, BisonSteakStats);
    }

    private static float EgocentrismCooldown(int stack) => LunarSunBehavior.secondsPerProjectile / stack;

    private static float GenesisLoopRecharge(int stack) => RechargeState.baseDuration / (stack + 1);

    private static float HeadstompersRecharge(int stack) => HeadstompersCooldown.baseDuration / stack;

    private static float UkuleleRange(int stack) => ItemValues.UkuleleRange + ItemValues.UkuleleRangePerStack * stack;

    private static float FireworksProcCoefficient() => ItemValues.FireworkProcCoefficient;

    private static float RunaldsBandProcCoefficient() => 1f;

    private static float AmplificationAsChance(float percent) => Util.ConvertAmplificationPercentageIntoReductionPercentage(percent) / 100f;

    private static ItemStatsDef TentabaubleStats() => new StatSheet()
        .Line("Root Chance: ", Style.Utility, Unit.Percentage)
        .Line("Root Duration: ", Style.Utility, Unit.Seconds)
        .Chance(Scaling.Hyperbolic, (luck, stack, procCoefficient) =>
        [
            StatMath.WithLuck(AmplificationAsChance(TentabaublePercentChancePerStack * stack * procCoefficient), luck),
            stack,
        ]);

    private static ItemStatsDef StunGrenadeStats() => new StatSheet()
        .Line("Stun Chance: ", Style.Utility, Unit.Percentage)
        .Chance(Scaling.Hyperbolic, (luck, stack, procCoefficient) =>
        [
            StatMath.WithLuck(AmplificationAsChance(SetStateOnHurt.stunChanceOnHitBaseChancePercent * stack * procCoefficient), luck),
        ]);

    private static ItemStatsDef SentientMeatHookStats() => new StatSheet()
        .Line("Hook Chance: ", Style.Damage, Unit.Percentage)
        .Line("Hook Damage: ", Style.Damage, Unit.Percentage)
        .Line("Max Targets: ", Style.Damage, Unit.Number)
        .Line("Radius: ", Style.Damage, Unit.Meters)
        .ProcCoefficient()
        .Chance(Scaling.Hyperbolic, (luck, stack, procCoefficient) =>
        [
            StatMath.WithLuck(StatMath.Hyperbolic(MeatHookChancePerStack, MeatHookChancePerStack, stack) * procCoefficient, luck),
            MeatHookDamage,
            MeatHookTargetsPerStack * (1 + stack),
            MeatHookRadius,
            MeatHookProcCoefficient,
        ]);

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
            StatMath.BoostMultiplier(StatMath.BoostItemsFor(seeds)),
        ];
    }

    private static ItemStatsDef LeptonDaisyStats() => new StatSheet()
        .Line("Healing Novas: ", Style.Healing, Unit.Number)
        .PerMaster((holder, stack) => [Holder.PlayerTeamItemCount(holder, RoR2Content.Items.TPHealingNova, stack)]);

    private static ItemStatsDef InfusionStats() => new StatSheet()
        .Line("Health On-Kill: ", Style.Healing, Unit.FlatHealth)
        .Line("Infusion Cap: ", Style.Healing, Unit.FlatHealth)
        .Line("Current Increase: ", Style.Healing, Unit.FlatHealth)
        .Line("Health Increase as %: ", Style.Healing, Unit.Percentage)
        .PerBody(InfusionValues);

    private static List<float> InfusionValues(CharacterBody body, int stack)
    {
        if (!body || !body.inventory)
            return null;

        var cap = InfusionCapPerStack * stack;

        return [stack, cap, body.inventory.infusionBonus, cap / Holder.LevelHealth(body)];
    }

    private static ItemStatsDef BisonSteakStats() => new StatSheet()
        .Line("Flat Health: ", Style.Healing, Unit.FlatHealth)
        .Line("Health Increase as %: ", Style.Healing, Unit.PercentHealth)
        .PerBody(BisonSteakValues);

    private static List<float> BisonSteakValues(CharacterBody body, int stack)
    {
        if (!body)
            return null;

        var health = BisonSteakHealthPerStack * stack;

        return [health, health / Holder.LevelHealth(body)];
    }
}
