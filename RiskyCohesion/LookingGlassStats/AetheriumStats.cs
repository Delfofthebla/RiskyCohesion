using System;
using Aetherium.Equipment;
using Aetherium.Items;
using Aetherium.Items.Tier1;
using Aetherium.Items.Tier2;
using Aetherium.Items.Tier3;
using Aetherium.Items.TierLunar;
using Aetherium.Utils;
using BepInEx.Configuration;
using LookingGlass.ItemStatsNameSpace;
using RiskyCohesion.ProcChains;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using Style = LookingGlass.ItemStatsNameSpace.ItemStatsDef.ValueType;
using Unit = LookingGlass.ItemStatsNameSpace.ItemStatsDef.MeasurementUnits;

namespace RiskyCohesion.LookingGlassStats;

internal static class AetheriumStats
{
    private static readonly float EngineersToolbeltMaxChance = 1f;
    private static readonly float SharkTeethMaxDelayedShare = 0.9f;
    private static readonly float UnmodifiedAttackSpeed = 1f;
    private static readonly float UnmodifiedProcCoefficient = 1f;
    private static readonly float InspiringDroneExplosionDamagePerStack = 3f;
    private static readonly float InspiringDroneExplosionBaseRadius = 12f;
    private static readonly float InspiringDroneExplosionRadiusPerStack = 2f;
    private static readonly float AlienMagnetPullSpeedPerStack = 5f;
    private static readonly float HoofMovementSpeedBonus = 0.14f;
    private static readonly float VoidImplosionProcCoefficient = 1f;
    private static readonly float BellRingDuration = 2f;
    private static readonly float JarReleasedDamageMultiplier = 2f;
    private static readonly float JarReleasedFlatDamage = 20f;

    public static void Install(ConfigFile config)
    {
        var description = new Description("Adds LookingGlass stats for Aetherium's items.");
        var stats = StatsGroup.Bind(config, ContentMod.Aetherium.Name, description);

        stats.Item(ItemDefOf<FeatheredPlume>, FeatheredPlumeStats);
        stats.Item(ItemDefOf<NailBomb>, NailBombStats);
        stats.Item(ItemDefOf<BloodthirstyShield>, BloodthirstyShieldStats);
        stats.Item(ItemDefOf<EngineersToolbelt>, EngineersToolbeltStats);
        stats.Item(ItemDefOf<SharkTeeth>, SharkTeethStats);
        stats.Item(ItemDefOf<ShieldingCore>, ShieldingCoreStats);
        stats.Item(ItemDefOf<BlasterSword>, BlasterSwordStats);
        stats.Item(ItemDefOf<InspiringDrone>, InspiringDroneStats);
        stats.Item(ItemDefOf<WitchesRing>, WitchesRingStats);
        stats.Item(ItemDefOf<AccursedPotion>, AccursedPotionStats);
        stats.Item(ItemDefOf<AlienMagnet>, AlienMagnetStats);
        stats.Item(ItemDefOf<UnstableDesign>, UnstableDesignStats);
        stats.Item(ItemDefOf<Voidheart>, VoidheartStats);
        stats.Item(ItemDefOf<WeightedAnklet>, WeightedAnkletStats);
        stats.Item(ItemDefOf<ZenithAccelerator>, ZenithAcceleratorStats);

        stats.Equipment(EquipmentDefOf<JarOfReshaping>, JarOfReshapingStats);
        stats.Equipment(EquipmentDefOf<BellTotem>, BellTotemStats);
    }

    private static ItemDef ItemDefOf<T>() where T : ItemBase<T> => ItemBase<T>.instance?.ItemDef;

    private static EquipmentDef EquipmentDefOf<T>() where T : EquipmentBase<T> => EquipmentBase<T>.instance?.EquipmentDef;

    private static ItemStatsDef FeatheredPlumeStats() => new StatSheet()
        .Line("Movement Speed Per Buff: ", Style.Utility, Unit.Percentage)
        .Line("Max Buffs: ", Style.Utility, Unit.Number)
        .Line("Max Movement Speed: ", Style.Utility, Unit.Percentage)
        .Line("Buff Duration: ", Style.Utility, Unit.Seconds)
        .PerStack(stack =>
        [
            FeatheredPlume.MoveSpeedPercentageBonusPerBuffStack,
            FeatheredPlumeMaxBuffs(stack),
            FeatheredPlume.MoveSpeedPercentageBonusPerBuffStack * FeatheredPlumeMaxBuffs(stack),
            StatMath.Linear(FeatheredPlume.BaseDurationOfBuffInSeconds, FeatheredPlume.AdditionalDurationOfBuffInSeconds, stack),
        ]);

    private static int FeatheredPlumeMaxBuffs(int stack) => FeatheredPlume.BuffStacksPerFeatheredPlume * stack;

    private static ItemStatsDef NailBombStats()
    {
        // Aetherium's alternate implementation only drops a bomb while its cooldown debuff is active, which nothing grants first, so it never drops one.
        if (NailBomb.UseAlternateImplementation)
            return null;

        var detonator = NailBomb.NailBombProjectileMain.GetComponent<NailBomb.NailBombDetonator>();

        return new StatSheet()
            .Line("Damage Threshold: ", Style.Damage, Unit.Percentage)
            .Line("Nails: ", Style.Damage, Unit.Number)
            .Line("Damage Per Nail: ", Style.Damage, Unit.Percentage)
            .Line("Per-Enemy Cooldown: ", Style.Utility, Unit.Seconds)
            .ProcCoefficient()
            .PerStack(stack =>
            [
                NailBomb.PercentDamageThresholdRequiredToActivate,
                detonator.NailCount,
                detonator.DamageCoefficient * StatMath.Linear(1f, NailBomb.PercentDamageBonusOfAdditionalStacks, stack),
                NailBomb.NailBombAbsurdityLimiterCooldown,
                AetheriumProcChains.NailBombNails.CoefficientInEffect(),
            ]);
    }

    private static ItemStatsDef BloodthirstyShieldStats() => new StatSheet()
        .Line("Shield Strength: ", Style.Utility, Unit.PercentHealth)
        .Line("Max Shield Restored On-Kill: ", Style.Healing, Unit.Percentage)
        .PerStack(stack => [BloodthirstyShield.BaseGrantShieldMultiplier, BloodthirstyShieldRestoredShare(stack)]);

    private static float BloodthirstyShieldRestoredShare(int stack)
        => MathHelpers.InverseHyperbolicScaling(
            BloodthirstyShield.ShieldPercentageRestoredPerKill,
            BloodthirstyShield.AdditionalShieldPercentageRestoredPerKillDiminishing,
            BloodthirstyShield.MaximumPercentageShieldRestoredPerKill,
            stack);

    private static ItemStatsDef EngineersToolbeltStats() => new StatSheet()
        .Line("Duplication Chance: ", Style.Utility, Unit.Percentage)
        .Line("Revival Chance: ", Style.Healing, Unit.Percentage)
        .WithLuck((luck, stack, _) =>
        [
            StatMath.WithLuck(EngineersToolbeltChance(EngineersToolbelt.BaseDuplicationPercentChance, EngineersToolbelt.AdditionalDuplicationPercentChance, stack), luck),
            StatMath.WithLuck(EngineersToolbeltChance(EngineersToolbelt.BaseRevivalPercentChance, EngineersToolbelt.AdditionalRevivalPercentChance, stack), luck),
        ]);

    private static float EngineersToolbeltChance(float first, float perStack, int stack) => MathHelpers.InverseHyperbolicScaling(first, perStack, EngineersToolbeltMaxChance, stack);

    private static ItemStatsDef SharkTeethStats()
    {
        var sheet = new StatSheet()
            .Line("Damage Taken Delayed: ", Style.Armor, Unit.Percentage);

        if (!SharkTeeth.EnableCleanseOnHit)
            return sheet.PerStack(stack => [SharkTeethDelayedShare(stack)]);

        return sheet
            .Line("Stored Damage Cleansed Per Skill Hit: ", Style.Healing, Unit.Percentage)
            .PerStack(stack => [SharkTeethDelayedShare(stack), SharkTeeth.CleansePercentage]);
    }

    private static float SharkTeethDelayedShare(int stack)
    {
        float cap = SharkTeeth.MaxDamageSpreadPercentage;
        var share = SharkTeeth.BaseDamageSpreadPercentage + cap - cap / (1f + SharkTeeth.AdditionalDamageSpreadPercentage * (stack - 1));

        return Mathf.Clamp(share, 0f, SharkTeethMaxDelayedShare);
    }

    private static ItemStatsDef ShieldingCoreStats() => new StatSheet()
        .Line("Armor While Shielded: ", Style.Armor, Unit.Number)
        .Line("Shield Strength: ", Style.Utility, Unit.PercentHealth)
        .PerStack(stack =>
        [
            StatMath.Linear(ShieldingCore.BaseShieldingCoreArmorGrant, ShieldingCore.AdditionalShieldingCoreArmorGrant, stack),
            ShieldingCore.BaseGrantShieldMultiplier,
        ]);

    private static ItemStatsDef BlasterSwordStats()
    {
        var sword = BlasterSword.SwordProjectile;
        var controller = sword.GetComponent<ProjectileController>();
        var attackProcCoefficient = BlasterSwordAttackProcCoefficient(sword);

        return new StatSheet()
            .Line("Sword Damage: ", Style.Damage, Unit.Percentage)
            .Line("Sword Cooldown: ", Style.Utility, Unit.Seconds)
            .ProcCoefficient()
            .PerBody((body, stack) =>
            [
                StatMath.Linear(BlasterSword.BaseSwordDamageMultiplier, BlasterSword.AdditionalSwordDamageMultiplier, stack),
                BlasterSwordCooldown(body ? body.attackSpeed : UnmodifiedAttackSpeed),
                controller.procCoefficient * attackProcCoefficient,
            ]);
    }

    private static float BlasterSwordCooldown(float attackSpeed) => Mathf.Max(BlasterSword.BlasterSwordBehavior.BaseCooldown / attackSpeed, BlasterSword.BlasterSwordBehavior.MinCooldown);

    private static float BlasterSwordAttackProcCoefficient(GameObject sword)
    {
        var impaleExplosion = sword.GetComponent<ProjectileImpactExplosion>();
        if (impaleExplosion)
            return impaleExplosion.blastProcCoefficient;

        var pierce = sword.GetComponent<ProjectileOverlapAttack>();
        return pierce ? pierce.overlapProcCoefficient : UnmodifiedProcCoefficient;
    }

    private static ItemStatsDef InspiringDroneStats()
    {
        var sheet = new StatSheet()
            .Line("Stats Inherited: ", Style.Utility, Unit.Percentage);

        if (!InspiringDrone.EnableDeathExplosion)
            return sheet.PerStack(stack => [InspiringDroneStatShare(stack)]);

        return sheet
            .Line("Death Explosion Damage: ", Style.Damage, Unit.Percentage)
            .Line("Death Explosion Radius: ", Style.Damage, Unit.Meters)
            .ProcCoefficient()
            .PerStack(stack =>
            [
                InspiringDroneStatShare(stack),
                InspiringDroneExplosionDamagePerStack * stack,
                InspiringDroneExplosionBaseRadius + InspiringDroneExplosionRadiusPerStack * stack,
                AetheriumProcChains.InspiringDroneExplosion.CoefficientInEffect(),
            ]);
    }

    private static float InspiringDroneStatShare(int stack) => InspiringDrone.AllStatValueGrantedPercentage * stack;

    private static ItemStatsDef WitchesRingStats() => new StatSheet()
        .Line("Damage Threshold: ", Style.Damage, Unit.Percentage)
        .Line(WitchesRing.GlobalCooldownOnUse ? "Recharge Time: " : "Per-Enemy Cooldown: ", Style.Utility, Unit.Seconds)
        .PerStack(stack =>
        [
            WitchesRing.WitchesRingTriggerThreshold,
            WitchesRing.BaseCooldownDuration / (1f + WitchesRing.AdditionalCooldownReduction * (stack - 1)),
        ]);

    private static ItemStatsDef AccursedPotionStats() => new StatSheet()
        .Line("Sip Cooldown: ", Style.Utility, Unit.Seconds)
        .Line("Sharing Radius: ", Style.Utility, Unit.Meters)
        .Line("Max Stacks Per Effect: ", Style.Utility, Unit.Number)
        .PerStack(stack =>
        [
            AccursedPotion.BaseSipCooldownDuration * Mathf.Pow(AccursedPotion.AdditionalStackSipCooldownReductionPercentage, stack - 1),
            StatMath.Linear(AccursedPotion.BaseRadiusGranted, AccursedPotion.AdditionalRadiusGranted, stack),
            stack,
        ]);

    private static ItemStatsDef AlienMagnetStats()
    {
        if (!AlienMagnet.UseOldFunctionality)
        {
            return new StatSheet()
                .Line("Pull Speed (m/s): ", Style.Utility, Unit.Number)
                .PerStack(stack => [AlienMagnetPullSpeedPerStack * stack]);
        }

        return new StatSheet()
            .Line("Pull Force Multiplier: ", Style.Utility, Unit.Number)
            .PerStack(stack => [AlienMagnetOldPullForce(stack)]);
    }

    private static float AlienMagnetOldPullForce(int stack)
        => Mathf.Clamp(
            StatMath.Linear(AlienMagnet.OldStartingForceMultiplier, AlienMagnet.OldAdditionalForceMultiplier, stack),
            AlienMagnet.OldMinimumForceMultiplier,
            AlienMagnet.OldMaximumForceMultiplier);

    private static ItemStatsDef UnstableDesignStats() => new StatSheet()
        .Line("Respawn Cooldown: ", Style.Utility, Unit.Seconds)
        .Line("Chimera Bonus Damage: ", Style.Damage, Unit.Percentage)
        .Line("Chimera Bonus Health: ", Style.Healing, Unit.Percentage)
        .Line("Chimera Bonus Attack Speed: ", Style.Damage, Unit.Percentage)
        .Line("Chimera Bonus Movement Speed: ", Style.Utility, Unit.Percentage)
        .PerStack(stack =>
        [
            UnstableDesign.LunarChimeraResummonCooldownDuration,
            // Matches Aetherium's grant, which subtracts 1 after multiplying by stacks, not from the stack count.
            StatMath.BoostBonus(UnstableDesign.LunarChimeraBaseDamageBoost + (UnstableDesign.LunarChimeraAdditionalDamageBoost * stack - 1)),
            StatMath.BoostBonus(UnstableDesign.LunarChimeraBaseHPBoost * stack),
            StatMath.BoostBonus(UnstableDesign.LunarChimeraBaseAttackSpeedBoost),
            HoofMovementSpeedBonus * UnstableDesign.LunarChimeraBaseMovementSpeedBoost * stack,
        ]);

    private static ItemStatsDef VoidheartStats() => new StatSheet()
        .Line("Implosion Damage: ", Style.Damage, Unit.Percentage)
        .Line("Implosion Radius: ", Style.Damage, Unit.Meters)
        .Line("Recharge Time: ", Style.Utility, Unit.Seconds)
        .Line("Healing Becomes Damage Below: ", Style.Death, Unit.PercentHealth)
        .ProcCoefficient()
        .PerStack(stack =>
        [
            Voidheart.VoidImplosionDamageMultiplier,
            StatMath.Linear(Voidheart.VoidImplosionBaseRadius, Voidheart.VoidImplosionAdditionalRadius, stack),
            Voidheart.VoidHeartCooldownDebuffDuration,
            VoidheartHealingThreshold(stack),
            VoidImplosionProcCoefficient,
        ]);

    private static float VoidheartHealingThreshold(int stack)
        => Mathf.Clamp(
            StatMath.Linear(Voidheart.VoidHeartBaseTickingTimeBombHealthThreshold, Voidheart.VoidHeartAdditionalTickingTimeBombHealthThreshold, stack),
            Voidheart.VoidHeartBaseTickingTimeBombHealthThreshold,
            Voidheart.VoidHeartMaxTickingTimeBombHealthThreshold);

    private static ItemStatsDef WeightedAnkletStats() => new StatSheet()
        .Line("Knockback Reduction: ", Style.Utility, Unit.Percentage)
        .Line("Attack Speed: ", Style.Death, Unit.Percentage)
        .Line("Movement Speed: ", Style.Death, Unit.Percentage)
        .Line("Base Attack Speed Per Removal: ", Style.Damage, Unit.Number)
        .Line("Base Movement Speed Per Removal: ", Style.Utility, Unit.Number)
        .Line("Damage Per Removal: ", Style.Damage, Unit.Percentage)
        .PerStack(stack =>
        [
            1f - Mathf.Clamp01(1f - stack * WeightedAnklet.BaseKnockbackReductionPercentage),
            -Mathf.Min(stack * WeightedAnklet.BaseAttackSpeedReductionPercentage, WeightedAnklet.AttackSpeedReductionPercentageCap),
            -Mathf.Min(stack * WeightedAnklet.BaseMovementSpeedReductionPercentage, WeightedAnklet.MovementSpeedReductionPercentageCap),
            WeightedAnklet.AttackSpeedGainedPerLimiterRelease,
            WeightedAnklet.MovementSpeedGainedPerLimiterRelease,
            WeightedAnklet.DamagePercentageGainedPerLimiterRelease,
        ]);

    private static ItemStatsDef ZenithAcceleratorStats() => new StatSheet()
        .Line("Min Attack Speed: ", Style.Death, Unit.Percentage)
        .Line("Max Attack Speed: ", Style.Damage, Unit.Percentage)
        .Line("Attack Speed Per Buff: ", Style.Damage, Unit.Percentage)
        .Line("Max Buffs: ", Style.Damage, Unit.Number)
        .PerStack(stack =>
        [
            ZenithMinAttackSpeed(stack),
            ZenithMaxAttackSpeed(stack),
            Zenith.PercentageIncreasePerHit,
            ZenithMaxBuffs(stack),
        ]);

    private static ZenithAccelerator Zenith => ZenithAccelerator.instance;

    private static float ZenithMinAttackSpeed(int stack) => Mathf.Pow(Zenith.ExponentialBaseMultiplier, stack);

    private static float ZenithMaxAttackSpeed(int stack) => StatMath.Linear(Zenith.BaseMaximumPercentageCap, Zenith.AdditionalMaximumPercentageCap, stack);

    private static float ZenithMaxBuffs(int stack) => (int)Math.Ceiling((ZenithMaxAttackSpeed(stack) - Math.Pow(Zenith.ExponentialBaseMultiplier, stack)) / Zenith.PercentageIncreasePerHit);

    private static ItemStatsDef JarOfReshapingStats() => new StatSheet()
        .Line("Absorb Radius: ", Style.Utility, Unit.Meters)
        .Line("Absorb Duration: ", Style.Utility, Unit.Seconds)
        .Line("Absorbed Damage Released: ", Style.Damage, Unit.Percentage)
        .Line("Bonus Damage Per Release: ", Style.Damage, Unit.Number)
        .PerStack(_ =>
        [
            JarOfReshaping.BaseRadiusGranted,
            JarOfReshaping.ProjectileAbsorptionTime,
            JarReleasedDamageMultiplier,
            JarReleasedFlatDamage,
        ]);

    private static ItemStatsDef BellTotemStats() => new StatSheet()
        .Line("Shockwave Radius: ", Style.Utility, Unit.Meters)
        .Line("Time Between Rings: ", Style.Utility, Unit.Seconds)
        .PerStack(_ => [BellTotem.RadiusOfBellRinging, BellTotem.TimeBetweenUses + BellRingDuration]);
}
