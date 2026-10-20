using BepInEx.Configuration;
using GrooveSaladSpikestripContent;
using GrooveSaladSpikestripContent.Content;
using LookingGlass.ItemStatsNameSpace;
using NoopSpikestrip;
using PlasmaCoreSpikestripContent.Content.Equipment;
using PlasmaCoreSpikestripContent.Content.Items;
using PlasmaCoreSpikestripContent.Misc;
using RiskyCohesion.ProcChains;
using RoR2;
using RoR2.Projectile;
using Spikestrip2_0.Core;
using UnityEngine;
using Scaling = LookingGlass.ItemStatsNameSpace.ItemStatsDef.ChanceScaling;
using SpikeFactory = GrooveSaladSpikestripContent.Content.PermaSpike.PermaSpikeBehaviourServer;
using Style = LookingGlass.ItemStatsNameSpace.ItemStatsDef.ValueType;
using Unit = LookingGlass.ItemStatsNameSpace.ItemStatsDef.MeasurementUnits;

namespace RiskyCohesion.LookingGlassStats;

internal static class SpikestripStats
{
    private static readonly float UnmodifiedAttackSpeed = 1f;

    private static readonly float PewterBraceletCritDamagePerHit = 0.1f;

    private static readonly float WoolArmorPerEnemy = 5f;
    private static readonly float WoolRadius = 20f;

    private static readonly float ChilledCo2FreezeDuration = 1.5f;
    private static readonly float ChilledCo2FreezeDurationPerStack = 0.5f;

    private static readonly float CoagulatedMixtureSlowDuration = 3f;
    private static readonly float CoagulatedMixtureSlowDurationPerStack = 1.5f;

    private static readonly float EssentialOilsRegenDuration = 0.5f;
    private static readonly float EssentialOilsRegenDurationPerStack = 0.25f;
    private static readonly float CrocoRegenHealingPerSecond = 0.1f;

    private static readonly float ArsonistsSatchelGasolines = 2f;
    private static readonly float ArsonistsSatchelGasolinesPerStack = 1f;

    private static readonly float SmoulderingCometRespawnTime = 10f;

    private static readonly float SunriseSmoothieFlatHealing = 4f;
    private static readonly float SunriseSmoothiePercentHealingPerStack = 0.01f;

    private static readonly float BloodySurgicalMaskBleedChancePerBuff = 0.05f;
    private static readonly float BloodySurgicalMaskMaxBuffs = 5f;
    private static readonly float BloodySurgicalMaskMaxBuffsPerStack = 3f;
    private static readonly float BloodySurgicalMaskBuffInterval = 2f;
    private static readonly float BloodySurgicalMaskCooldown = 5f;

    private static readonly float BloodOrangeDotProcCoefficientPerStack = 0.5f;
    private static readonly float BloodOrangeDamageThreshold = 4f;
    private static readonly float BloodOrangeDotDuration = 3f;

    private static readonly float SunBladeRechargeTime = 7f;
    private static readonly float SunBladeFollowUpDuration = 30f;
    private static readonly float SunBladeFollowUpIgniteDamagePerStack = 0.5f;

    private static readonly float BoxOfAgonyArmor = 40f;
    private static readonly float BoxOfAgonyArmorPerStack = 10f;

    private static readonly float ImperfectConstructShieldShare = 0.4f;
    private static readonly float ImperfectConstructShieldShareReductionPerStack = 0.1f;
    private static readonly float ImperfectConstructDamageThreshold = 75f;
    private static readonly float ImperfectConstructDamageThresholdReductionPerStack = 0.15f;

    private static readonly float AbyssalCartridgeDebuffsPerItem = 10f;
    private static readonly float AbyssalCartridgeBlessingChance = 0.1f;

    private static readonly float BlessingStatBonusPerStack = 0.03f;

    private static readonly float CourierSphereRange = 150f;
    private static readonly float CourierSphereProcCoefficient = 1f;

    private static readonly float ImpulseFrostShieldFreezeDuration = 4f;
    private static readonly float ImpulseFrostShieldHealthDamage = 0.1f;
    private static readonly float ImpulseFrostShieldRange = 225f;

    public static void Install(ConfigFile config)
    {
        var description = new Description("Adds LookingGlass stats for Spikestrip 2.0's items.");
        var stats = StatsGroup.Bind(config, ContentMod.Spikestrip.Name, description);

        stats.Item(ItemDefOf<Bracelet>, PewterBraceletStats);
        stats.Item(ItemDefOf<Calendar>, GroovesAlarmStats);
        stats.Item(ItemDefOf<Wool>, WoolStats);
        stats.Item(ItemDefOf<GasolineFreeze>, ChilledCo2Stats);
        stats.Item(ItemDefOf<GasolineGup>, CoagulatedMixtureStats);
        stats.Item(ItemDefOf<GasolineHeal>, EssentialOilsStats);
        stats.Item(ItemDefOf<GasolinePressure>, PressurizedFluidStats);
        stats.Item(ItemDefOf<Arson>, ArsonistsSatchelStats);
        stats.Item(ItemDefOf<BlueLemurian>, SmoulderingCometStats);
        stats.Item(ItemDefOf<HealOnSkillUse>, SunriseSmoothieStats);
        stats.Item(ItemDefOf<BloodyMask>, BloodySurgicalMaskStats);
        stats.Item(ItemDefOf<DotProcs>, BloodOrangeStats);
        stats.Item(ItemDefOf<InfinityCodex>, InfinityCodexStats);
        stats.Item(ItemDefOf<PermaSpike>, PermaSpikeFactoryStats);
        stats.Item(ItemDefOf<SunSword>, SunBladeStats);
        stats.Item(ItemDefOf<PainBox>, BoxOfAgonyStats);
        stats.Item(ItemDefOf<Singularity>, CosmicBulwarkStats);
        stats.Item(ItemDefOf<ObligiatoryReference>, ImperfectConstructStats);
        stats.Item(ItemDefOf<Voiditem>, AbyssalCartridgeStats);
        stats.Item(ItemDefOf<BLESSING>, BlessingStats);

        stats.Equipment(EquipmentDefOf<MultiTargetEquip>, CourierSphereStats);
        stats.Equipment(EquipmentDefOf<DoubleItemsEquip>, QuartzRabbitStats);
        stats.Equipment(EquipmentDefOf<ImpulseFrostShield>, ImpulseFrostShieldStats);

        RoR2Application.onLoad += () => SpikestripBloodOrange.OnReworkToggled(stats.Refresh);
    }

    private static ItemDef ItemDefOf<T>() where T : ItemBase<T> => SpikestripContent.IsEnabled<T>()
        ? ItemBase<T>.instance.ItemDef
        : null;

    private static EquipmentDef EquipmentDefOf<T>() where T : EquipmentBase<T> => SpikestripContent.IsEnabled<T>()
        ? EquipmentBase<T>.instance.EquipmentDef
        : null;

    private static ItemStatsDef PewterBraceletStats() => new StatSheet()
        .Line("Crit Damage Per Hit: ", Style.Damage, Unit.Percentage)
        .PerStack(stack => [PewterBraceletCritDamagePerHit * stack]);

    private static ItemStatsDef GroovesAlarmStats() => new StatSheet()
        .Line("Daily Bonus: ", Style.Utility, Unit.Percentage)
        .PerStack(stack => [Calendar.bonusPower * stack]);

    private static ItemStatsDef WoolStats() => new StatSheet()
        .Line("Armor Per Nearby Enemy: ", Style.Armor, Unit.Number)
        .Line("Radius: ", Style.Utility, Unit.Meters)
        .PerStack(stack => [WoolArmorPerEnemy * stack, WoolRadius]);

    private static ItemStatsDef ChilledCo2Stats()
    {
        var gasoline = GasolineFreeze.instance;

        return new StatSheet()
            .Line("Freeze Chance: ", Style.Utility, Unit.Percentage)
            .Line("Freeze Duration: ", Style.Utility, Unit.Seconds)
            .Line("Radius: ", Style.Utility, Unit.Meters)
            .Line("Explosion Damage: ", Style.Damage, Unit.Percentage)
            .ProcCoefficient()
            .WithLuck((luck, stack, _) =>
            [
                GasolineChance(gasoline, luck, stack),
                StatMath.Linear(ChilledCo2FreezeDuration, ChilledCo2FreezeDurationPerStack, stack),
                GasolineRadius(gasoline, stack),
                GasolineDamage(gasoline, stack),
                gasoline.ProcCoefficient,
            ]);
    }

    private static ItemStatsDef CoagulatedMixtureStats()
    {
        var gasoline = GasolineGup.instance;

        return new StatSheet()
            .Line("Slow Duration: ", Style.Utility, Unit.Seconds)
            .Line("Radius: ", Style.Utility, Unit.Meters)
            .Line("Explosion Damage: ", Style.Damage, Unit.Percentage)
            .ProcCoefficient()
            .PerStack(stack =>
            [
                StatMath.Linear(CoagulatedMixtureSlowDuration, CoagulatedMixtureSlowDurationPerStack, stack),
                GasolineRadius(gasoline, stack),
                GasolineDamage(gasoline, stack),
                gasoline.ProcCoefficient,
            ]);
    }

    private static ItemStatsDef EssentialOilsStats()
    {
        var gasoline = GasolineHeal.instance;

        return new StatSheet()
            .Line("Healing: ", Style.Healing, Unit.PercentHealing)
            .Line("Radius: ", Style.Utility, Unit.Meters)
            .PerStack(stack => [CrocoRegenHealingPerSecond * StatMath.Linear(EssentialOilsRegenDuration, EssentialOilsRegenDurationPerStack, stack), GasolineRadius(gasoline, stack)]);
    }

    private static ItemStatsDef PressurizedFluidStats()
    {
        var gasoline = GasolinePressure.instance;

        return new StatSheet()
            .Line("Pull Chance: ", Style.Utility, Unit.Percentage)
            .Line("Radius: ", Style.Utility, Unit.Meters)
            .Line("Explosion Damage: ", Style.Damage, Unit.Percentage)
            .ProcCoefficient()
            .WithLuck((luck, stack, _) => [GasolineChance(gasoline, luck, stack), GasolineRadius(gasoline, stack), GasolineDamage(gasoline, stack), gasoline.ProcCoefficient]);
    }

    private static float GasolineChance<T>(SpikestripGasolineBase<T> gasoline, float luck, int stack) where T : SpikestripGasolineBase<T>
        => StatMath.WithLuck(Mathf.Min(StatMath.Linear(gasoline.ProcChanceBase, gasoline.ProcChanceStack, stack) / 100f, 1f), luck);

    private static float GasolineRadius<T>(SpikestripGasolineBase<T> gasoline, int stack) where T : SpikestripGasolineBase<T> => StatMath.Linear(gasoline.RadiusBase, gasoline.RadiusStack, stack);

    private static float GasolineDamage<T>(SpikestripGasolineBase<T> gasoline, int stack) where T : SpikestripGasolineBase<T>
        => StatMath.Linear(gasoline.ExplosionDamageBase, gasoline.ExplosionDamageStack, stack);

    private static ItemStatsDef ArsonistsSatchelStats() => new StatSheet()
        .Line("Gasolines Per Stage: ", Style.Utility, Unit.Number)
        .PerStack(stack => [StatMath.Linear(ArsonistsSatchelGasolines, ArsonistsSatchelGasolinesPerStack, stack)]);

    private static ItemStatsDef SmoulderingCometStats() => new StatSheet()
        .Line("Lemurian Bonus Damage: ", Style.Damage, Unit.Percentage)
        .Line("Lemurian Bonus Health: ", Style.Healing, Unit.Percentage)
        .Line("Respawn Time: ", Style.Utility, Unit.Seconds)
        .PerStack(stack =>
        [
            BlueLemurianBonus(BlueLemurian.damageBonusBase, BlueLemurian.damageBonusPerStack, stack),
            BlueLemurianBonus(BlueLemurian.healthBonusBase, BlueLemurian.healthBonusPerStack, stack),
            SmoulderingCometRespawnTime,
        ]);

    private static float BlueLemurianBonus(float bonus, float bonusPerStack, int stack) => (int)(bonus + bonusPerStack * stack);

    private static ItemStatsDef SunriseSmoothieStats() => new StatSheet()
        .Line("Flat Healing: ", Style.Healing, Unit.FlatHealing)
        .Line("Percent Healing: ", Style.Healing, Unit.PercentHealing)
        .PerStack(stack => [SunriseSmoothieFlatHealing, SunriseSmoothiePercentHealingPerStack * stack]);

    private static ItemStatsDef BloodySurgicalMaskStats() => new StatSheet()
        .Line("Max Bleed Chance: ", Style.Damage, Unit.Percentage)
        .Line("Max Buffs: ", Style.Damage, Unit.Number)
        .Line("Buff Interval: ", Style.Utility, Unit.Seconds)
        .Line("Cooldown When Hit: ", Style.Utility, Unit.Seconds)
        .Chance(Scaling.Linear, (luck, stack, procCoefficient) =>
        [
            StatMath.WithLuck(BloodySurgicalMaskBleedChancePerBuff * BloodySurgicalMaskMaxBuffCount(stack) * procCoefficient, luck),
            BloodySurgicalMaskMaxBuffCount(stack),
            BloodySurgicalMaskBuffInterval,
            BloodySurgicalMaskCooldown,
        ]);

    private static float BloodySurgicalMaskMaxBuffCount(int stack) => StatMath.Linear(BloodySurgicalMaskMaxBuffs, BloodySurgicalMaskMaxBuffsPerStack, stack);

    private static ItemStatsDef BloodOrangeStats() => SpikestripBloodOrange.IsReworked
        ? ReworkedBloodOrangeStats()
        : OriginalBloodOrangeStats();

    private static ItemStatsDef ReworkedBloodOrangeStats() => new StatSheet()
        .Line("Bonus Per Other DoT Type: ", Style.Damage, Unit.Percentage)
        .Line("Damage Threshold: ", Style.Damage, Unit.Percentage)
        .Line("Bonus DoT Damage: ", Style.Damage, Unit.Percentage)
        .PerStack(stack => [SpikestripBloodOrange.Bonus(stack), BloodOrangeDamageThreshold, BloodOrangeDotDamage()]);

    private static ItemStatsDef OriginalBloodOrangeStats() => new StatSheet()
        .Line("DoT Proc Coefficient: ", Style.Damage, Unit.ProcCoeff)
        .Line("Damage Threshold: ", Style.Damage, Unit.Percentage)
        .Line("Bonus DoT Damage: ", Style.Damage, Unit.Percentage)
        .PerStack(stack => [BloodOrangeDotProcCoefficientPerStack * stack, BloodOrangeDamageThreshold, BloodOrangeDotDamage()]);

    private static float BloodOrangeDotDamage()
    {
        var orangeDot = DotController.GetDotDef(DotProcs.bloodOrangeDotIndex);

        return orangeDot.damageCoefficient * BloodOrangeDotDuration / orangeDot.interval;
    }

    private static ItemStatsDef InfinityCodexStats() => new StatSheet()
        .Line("Damage To Logged Monsters: ", Style.Damage, Unit.Percentage)
        .PerStack(stack => [ItemValues.InfinityCodexDamagePerStack * stack]);

    private static ItemStatsDef PermaSpikeFactoryStats()
    {
        var controller = PermaSpike.projectilePrefab.GetComponent<ProjectileController>();
        var zone = PermaSpike.projectilePrefab.GetComponent<PermaSpike.PermaSpikeDotZone>();

        return new StatSheet()
            .Line("Spike Pile Interval: ", Style.Utility, Unit.Seconds)
            .Line("Damage Per Hit: ", Style.Damage, Unit.Percentage)
            .Line("Hits Per Pile: ", Style.Damage, Unit.Number)
            .Line("Pile Lifetime: ", Style.Utility, Unit.Seconds)
            .ProcCoefficient()
            .PerBody((body, stack) =>
            [
                SpikeFactory.baseFireInterval / (body ? body.attackSpeed : UnmodifiedAttackSpeed),
                zone.damageCoefficient * StatMath.Linear(SpikeFactory.damageCoef, SpikeFactory.stackingDamageCoef, stack),
                zone.maxHits,
                zone.lifetime,
                controller.procCoefficient * zone.overlapProcCoefficient,
            ]);
    }

    private static ItemStatsDef SunBladeStats() => new StatSheet()
        .Line("Ignite Damage: ", Style.Damage, Unit.Percentage)
        .Line("Recharge Time: ", Style.Utility, Unit.Seconds)
        .Line("Follow-Up Duration: ", Style.Utility, Unit.Seconds)
        .Line("Follow-Up Ignite Damage: ", Style.Damage, Unit.Percentage)
        .PerStack(stack => [SunSwordOrb.sunSwordDamageMult, SunBladeRechargeTime, SunBladeFollowUpDuration, SunBladeFollowUpIgniteDamagePerStack * stack]);

    private static ItemStatsDef BoxOfAgonyStats() => new StatSheet()
        .Line("Armor: ", Style.Armor, Unit.Number)
        .PerStack(stack => [StatMath.Linear(BoxOfAgonyArmor, BoxOfAgonyArmorPerStack, stack)]);

    private static ItemStatsDef CosmicBulwarkStats() => new StatSheet()
        .Line("Items Per Player: ", Style.Utility, Unit.Number)
        .PerStack(_ => [Singularity.DistributeSingularityItemsServer.maxItemCount]);

    private static ItemStatsDef ImperfectConstructStats() => new StatSheet()
        .Line("Health As Shield: ", Style.Health, Unit.PercentHealth)
        .Line("Damage To Recharge: ", Style.Damage, Unit.Percentage)
        .Line("Shield Recharged: ", Style.Healing, Unit.PercentHealth)
        .PerStack(stack =>
        [
            StatMath.ReducedPerStack(ImperfectConstructShieldShare, ImperfectConstructShieldShareReductionPerStack, stack),
            StatMath.ReducedPerStack(ImperfectConstructDamageThreshold, ImperfectConstructDamageThresholdReductionPerStack, stack),
            ObligiatoryReference.ReferenceBehavior.rechargeFraction,
        ]);

    private static ItemStatsDef AbyssalCartridgeStats() => new StatSheet()
        .Line("Debuffs Prevented: ", Style.Utility, Unit.Number)
        .Line("Blessing Chance: ", Style.Utility, Unit.Percentage)
        .WithLuck((luck, stack, _) => [AbyssalCartridgeDebuffsPerItem * stack, StatMath.WithLuck(AbyssalCartridgeBlessingChance, luck)]);

    private static ItemStatsDef BlessingStats() => new StatSheet()
        .Line("All Stats: ", Style.Utility, Unit.Percentage)
        .PerStack(stack => [BlessingStatBonusPerStack * stack]);

    private static ItemStatsDef CourierSphereStats() => new StatSheet()
        .Line("Damage Per Target: ", Style.Damage, Unit.Percentage)
        .Line("Range: ", Style.Utility, Unit.Meters)
        .ProcCoefficient()
        .PerStack(_ => [MultiTargetEquip.damageCoef, CourierSphereRange, CourierSphereProcCoefficient]);

    private static ItemStatsDef QuartzRabbitStats() => new StatSheet()
        .Line("Duration: ", Style.Utility, Unit.Seconds)
        .PerStack(_ => [DoubleItemsEquip.buffDuration]);

    private static ItemStatsDef ImpulseFrostShieldStats() => new StatSheet()
        .Line("Freeze Duration: ", Style.Utility, Unit.Seconds)
        .Line("Damage: ", Style.Damage, Unit.PercentHealth)
        .Line("Range: ", Style.Utility, Unit.Meters)
        .PerStack(_ => [ImpulseFrostShieldFreezeDuration, ImpulseFrostShieldHealthDamage, ImpulseFrostShieldRange]);
}
