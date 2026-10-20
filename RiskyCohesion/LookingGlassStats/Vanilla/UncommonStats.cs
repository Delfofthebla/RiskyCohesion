using System.Collections.Generic;
using LookingGlass.ItemStatsNameSpace;
using RiskyCohesion.Sources;
using RiskyMod.Items;
using RiskyMod.Items.DLC1.Uncommon;
using RiskyMod.Items.DLC2;
using RiskyMod.Items.DLC3.Uncommon;
using RiskyMod.Items.Uncommon;
using RoR2;
using UnityEngine;
using Scaling = LookingGlass.ItemStatsNameSpace.ItemStatsDef.ChanceScaling;
using Style = LookingGlass.ItemStatsNameSpace.ItemStatsDef.ValueType;
using Unit = LookingGlass.ItemStatsNameSpace.ItemStatsDef.MeasurementUnits;

namespace RiskyCohesion.LookingGlassStats.Vanilla;

internal static class UncommonStats
{
    private static readonly float DamageTakenMinimumChance = 0.05f;
    private static readonly float DamageTakenCertainChance = 0.75f;
    private static readonly float InfusionHealthPerTier = 100f;
    private static readonly float InfusionTierCostIncrease = 150f;
    private static readonly int HarpoonMaxStacks = 25;
    private static readonly float HarpoonBonusDuration = 0.5f;

    public static void Register(StatsGroup stats)
    {
        if (RiskyModChanges(Chronobauble.enabled))
            stats.AmendItem(() => RoR2Content.Items.SlowOnHit, ChronobaubleStats);

        if (RiskyModChanges(LeechingSeed.enabled))
            stats.Item(() => RoR2Content.Items.Seed, LeechingSeedStats);

        if (RiskyModChanges(AtG.enabled))
            stats.Item(() => RoR2Content.Items.Missile, AtgMissileStats);

        if (RiskyModChanges(ElementalBands.enabled))
        {
            stats.Item(() => RoR2Content.Items.FireRing, KjarosBandStats);
            stats.Item(() => RoR2Content.Items.IceRing, RunaldsBandStats);
        }

        if (RiskyModChanges(Stealthkit.enabled))
            stats.Item(() => RoR2Content.Items.Phasing, OldWarStealthkitStats);

        if (RiskyModChanges(WillOWisp.enabled))
            stats.Item(() => RoR2Content.Items.ExplodeOnDeath, WillOTheWispStats);

        if (RiskyModChanges(SquidPolyp.enabled))
            stats.Item(() => RoR2Content.Items.Squid, SquidPolypStats);

        if (RiskyModSource.ChangesUkulele)
            stats.AmendItem(() => RoR2Content.Items.ChainLightning, UkuleleStats);

        if (RiskyModChanges(Razorwire.enabled))
            stats.Item(() => RoR2Content.Items.Thorns, RazorwireStats);

        if (RiskyModChanges(RoseBuckler.enabled))
            stats.Item(() => RoR2Content.Items.SprintArmor, RoseBucklerStats);

        if (RiskyModChanges(Guillotine.enabled))
            stats.Item(() => RoR2Content.Items.ExecuteLowHealthElite, OldGuillotineStats);

        if (RiskyModChanges(Berzerker.enabled))
            stats.Item(() => RoR2Content.Items.WarCryOnMultiKill, BerzerkersPauldronStats);

        if (RiskyModChanges(WarHorn.enabled))
            stats.AmendItem(() => RoR2Content.Items.EnergizedOnEquipmentUse, WarHornStats);

        if (RiskyModChanges(Infusion.enabled))
            stats.Item(() => RoR2Content.Items.Infusion, InfusionStats);

        if (RiskyModChanges(RedWhip.enabled))
            stats.Item(() => RoR2Content.Items.SprintOutOfCombat, RedWhipStats);

        if (RiskyModChanges(Daisy.enabled))
            stats.Item(() => RoR2Content.Items.TPHealingNova, LeptonDaisyStats);

        if (RiskyModChanges(Harpoon.enabled))
            stats.Item(() => DLC1Content.Items.MoveSpeedOnKill, HuntersHarpoonStats);

        if (RiskyModChanges(Ignition.enabled))
            stats.Item(() => DLC1Content.Items.StrengthenBurn, IgnitionTankStats);

        if (RiskyModChanges(BreachingFin.enabled) && RiskyFixesSource.FixesBreachingFinReproc)
            stats.Item(() => DLC2Content.Items.KnockBackHitEnemies, BreachingFinStats);

        if (RiskyModChanges(NoxiousThorn.enabled))
            stats.AmendItem(() => DLC2Content.Items.TriggerEnemyDebuffs, NoxiousThornStats);

        if (RiskyModChanges(Dynamite.enabled))
            stats.Item(() => DLC3Content.Items.DronesDropDynamite, BoxOfDynamiteStats);

        if (RiskyModChanges(KineticDampener.enabled))
            stats.Item(() => DLC3Content.Items.ShieldBooster, KineticDampenerStats);
    }

    private static bool RiskyModChanges(bool itemEnabled) => VanillaStats.RiskyModChanges(ItemsCore.uncommonEnabled, itemEnabled);

    private static ItemStatsDef ChronobaubleStats(ItemStatsDef original)
        => original.WithLine("Attack Speed Reduction: ", Style.Utility, Unit.Percentage, () => 1f - 1f / 1.25f).WithLine("Cooldown Increase: ", Style.Utility, Unit.Percentage, () => 0.2f);

    private static ItemStatsDef LeechingSeedStats() => new StatSheet()
        .Line("Healing On-Hit: ", Style.Healing, Unit.FlatHealth)
        .Line("Healing From Damage: ", Style.Healing, Unit.Percentage)
        .Chance(Scaling.Health, (_, stack, procCoefficient) => [1f, 0.01f * stack * procCoefficient]);

    private static ItemStatsDef AtgMissileStats() => new StatSheet()
        .Line("Proc Chance: ", Style.Damage, Unit.Percentage)
        .Line("Total Damage: ", Style.Damage, Unit.Percentage)
        .ProcCoefficient()
        .Chance(Scaling.DoesNotScale, (luck, stack, procCoefficient) =>
        [
            StatMath.WithLuck(0.1f * procCoefficient, luck),
            StatMath.Linear(AtG.initialDamageCoefficient, AtG.stackDamageCoefficient, stack),
            VanillaStats.ProcCoefficient(1f),
        ]);

    private static ItemStatsDef KjarosBandStats() => new StatSheet()
        .Line("Total Damage: ", Style.Damage, Unit.Percentage)
        .ProcCoefficient()
        .PerStack(stack =>
        [
            StatMath.Linear(ElementalBands.initialDamageCoefficientFire, ElementalBands.stackDamageCoefficientFire, stack),
            0f,
        ]);

    private static ItemStatsDef RunaldsBandStats() => new StatSheet()
        .Line("Slow Duration: ", Style.Utility, Unit.Seconds)
        .Line("Total Damage: ", Style.Damage, Unit.Percentage)
        .ProcCoefficient()
        .PerStack(stack =>
        [
            3f * stack,
            StatMath.Linear(ElementalBands.initialDamageCoefficientIce, ElementalBands.stackDamageCoefficientIce, stack),
            1f,
        ]);

    private static ItemStatsDef OldWarStealthkitStats() => new StatSheet()
        .Line("Minimum Chance: ", Style.Utility, Unit.Percentage)
        .Line("Guaranteed at Health Lost: ", Style.Health, Unit.PercentHealth)
        .Line("Buff Duration: ", Style.Utility, Unit.Seconds)
        .WithLuck((luck, stack, _) =>
        [
            StatMath.WithLuck(DamageTakenMinimumChance, luck),
            DamageTakenCertainChance,
            StatMath.Linear(3f, 1.5f, stack),
        ]);

    private static ItemStatsDef WillOTheWispStats() => new StatSheet()
        .Line("Radius: ", Style.Damage, Unit.Meters)
        .Line("Pillar Damage: ", Style.Damage, Unit.Percentage)
        .ProcCoefficient()
        .PerStack(stack => [16f, StatMath.Linear(3.5f, 2.8f, stack), VanillaStats.ProcCoefficient(1f)]);

    private static ItemStatsDef SquidPolypStats() => new StatSheet()
        .Line("Minimum Chance: ", Style.Utility, Unit.Percentage)
        .Line("Guaranteed at Health Lost: ", Style.Health, Unit.PercentHealth)
        .Line("Max Turrets: ", Style.Utility, Unit.Number)
        .Line("Attack Speed: ", Style.Damage, Unit.Percentage)
        .Line("Lifetime: ", Style.Utility, Unit.Seconds)
        .PerMaster(SquidPolypValues);

    private static List<float> SquidPolypValues(CharacterMaster holder, int stack)
    {
        var luck = holder ? holder.luck : 0f;
        var heldByPlayers = (!holder || holder.teamIndex == TeamIndex.Player) && !RiskyMod.SoftDependencies.KingKombatArenaActive;
        var maxTurrets = SquidPolyp.scaleCount
            ? 2 + stack
            : 3;

        return
        [
            StatMath.WithLuck(DamageTakenMinimumChance, luck),
            DamageTakenCertainChance,
            maxTurrets,
            stack,
            heldByPlayers ? 40f : 20f,
        ];
    }

    private static ItemStatsDef UkuleleStats(ItemStatsDef original) => original.WithValue("Chain Distance: ", () => 25f);

    private static ItemStatsDef RazorwireStats() => new StatSheet()
        .Line("Targets: ", Style.Damage, Unit.Number)
        .Line("Radius: ", Style.Damage, Unit.Meters)
        .Line("Damage: ", Style.Damage, Unit.Percentage)
        .ProcCoefficient()
        .PerStack(stack => [1 + 2 * stack, 25f, StatMath.Linear(1.6f, 0.8f, stack), 0.5f]);

    private static ItemStatsDef RoseBucklerStats() => new StatSheet()
        .Line("Armor: ", Style.Armor, Unit.Number)
        .Line("Armor While Sprinting: ", Style.Armor, Unit.Number)
        .PerStack(stack => [10 * stack, 30 * stack]);

    private static ItemStatsDef OldGuillotineStats() => new StatSheet()
        .Line("Threshold: ", Style.Health, Unit.PercentHealth)
        .Line("Bonus Damage: ", Style.Damage, Unit.Percentage)
        .PerStack(stack => [0.5f, 0.3f * stack]);

    private static ItemStatsDef BerzerkersPauldronStats() => new StatSheet()
        .Line("Buff Duration: ", Style.Utility, Unit.Seconds)
        .Line("Max Stacks: ", Style.Utility, Unit.Number)
        .Line("Max Attack Speed: ", Style.Damage, Unit.Percentage)
        .Line("Max Movement Speed: ", Style.Utility, Unit.Percentage)
        .PerStack(BerzerkersPauldronValues);

    private static List<float> BerzerkersPauldronValues(int stack)
    {
        var maxStacks = 1 + 2 * stack;
        var maxSpeed = 0.15f * maxStacks;

        return [6f, maxStacks, maxSpeed, maxSpeed];
    }

    private static ItemStatsDef WarHornStats(ItemStatsDef original) => original.WithLine("Movement Speed: ", Style.Utility, Unit.Percentage, () => 0.5f);

    private static ItemStatsDef InfusionStats() => new StatSheet()
        .Line("Health On-Kill: ", Style.Healing, Unit.FlatHealth)
        .Line("Current Increase: ", Style.Healing, Unit.FlatHealth)
        .Line("Health Increase as %: ", Style.Healing, Unit.Percentage)
        .PerBody(InfusionValues);

    private static List<float> InfusionValues(CharacterBody body, int stack)
    {
        if (!body || !body.inventory || body.baseMaxHealth <= 0f)
            return null;

        var (health, tierCost) = InfusionProgress(body.inventory.infusionBonus);
        var increase = Mathf.Floor(health);

        return [stack * InfusionHealthPerTier / tierCost, increase, increase / Holder.LevelHealth(body)];
    }

    private static (float Health, float TierCost) InfusionProgress(float points)
    {
        var health = 0f;
        var tierCost = InfusionHealthPerTier;

        while (points >= tierCost)
        {
            points -= tierCost;
            health += InfusionHealthPerTier;
            tierCost += InfusionTierCostIncrease;
        }

        return (health + InfusionHealthPerTier * points / tierCost, tierCost);
    }

    private static ItemStatsDef RedWhipStats() => new StatSheet()
        .Line("Speed Bonus: ", Style.Utility, Unit.Percentage)
        .Line("Movement Speed: ", Style.Utility, Unit.Percentage)
        .PerStack(stack => [0.3f * stack, 0.1f * stack]);

    private static ItemStatsDef LeptonDaisyStats()
    {
        var sheet = new StatSheet();

        if (!Daisy.disableHealPulse)
            sheet.Line("Healing Novas: ", Style.Healing, Unit.Number);

        return sheet
            .Line("Healing: ", Style.Healing, Unit.PercentHealing)
            .Line("Radius: ", Style.Utility, Unit.Meters)
            .PerMaster(LeptonDaisyValues);
    }

    private static List<float> LeptonDaisyValues(CharacterMaster holder, int stack)
    {
        var daisies = Holder.PlayerTeamItemCount(holder, RoR2Content.Items.TPHealingNova, stack);
        List<float> ward = [0.05f + 0.025f * Mathf.Max(0, daisies - 1), 16f];

        return Daisy.disableHealPulse
            ? ward
            : [daisies, .. ward];
    }

    private static ItemStatsDef HuntersHarpoonStats() => new StatSheet()
        .Line("Buff Duration: ", Style.Utility, Unit.Seconds)
        .Line("Max Buff Duration: ", Style.Utility, Unit.Seconds)
        .Line("Movement Speed: ", Style.Utility, Unit.Percentage)
        .PerStack(stack =>
        [
            Mathf.Min(HarpoonMaxStacks, stack) + HarpoonBonusDuration,
            HarpoonMaxStacks + HarpoonBonusDuration,
            0.5f,
        ]);

    private static ItemStatsDef IgnitionTankStats() => new StatSheet()
        .Line("Ignite Chance: ", Style.Damage, Unit.Percentage)
        .Line("Total Damage: ", Style.Damage, Unit.Percentage)
        .Line("Bonus Damage: ", Style.Damage, Unit.Percentage)
        .Chance(Scaling.DoesNotScale, (luck, stack, procCoefficient) =>
        [
            StatMath.WithLuck(0.05f * procCoefficient, luck),
            0.5f,
            3f * stack,
        ]);

    private static ItemStatsDef BreachingFinStats() => new StatSheet()
        .Line("Max Launch Count: ", Style.Utility, Unit.Number)
        .Line("Max Damage Bonus: ", Style.Damage, Unit.Percentage)
        .PerStack(BreachingFinValues);

    private static List<float> BreachingFinValues(int stack)
    {
        var relaunches = 1 + stack;
        var launches = 1 + relaunches;

        return [relaunches, 0.15f * launches];
    }

    private static ItemStatsDef NoxiousThornStats(ItemStatsDef original) => original.WithValue("Range: ", () => 25f);

    private static ItemStatsDef BoxOfDynamiteStats() => new StatSheet()
        .Line("Damage: ", Style.Damage, Unit.Percentage)
        .ProcCoefficient()
        .PerStack(stack => [StatMath.Linear(2.4f, 1.2f, stack), 1f]);

    private static ItemStatsDef KineticDampenerStats() => new StatSheet()
        .Line("Armor: ", Style.Armor, Unit.Number)
        .Line("Shield Strength: ", Style.Utility, Unit.PercentHealth)
        .Line("Shield Explosion Damage: ", Style.Damage, Unit.Percentage)
        .Line("Radius: ", Style.Damage, Unit.Meters)
        .ProcCoefficient()
        .PerBody(KineticDampenerValues);

    private static List<float> KineticDampenerValues(CharacterBody body, int stack)
    {
        if (!body || body.damage <= 0f)
            return null;

        var shieldDamage = body.maxShield * StatMath.Linear(1f, 0.5f, stack);

        return [30 * stack, 0.1f, 1f + shieldDamage / body.damage, 20f, 1f];
    }
}
