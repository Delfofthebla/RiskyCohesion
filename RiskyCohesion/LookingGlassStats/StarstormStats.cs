using System.Collections.Generic;
using BepInEx.Configuration;
using LookingGlass.ItemStatsNameSpace;
using MSU;
using RiskyCohesion.ProcChains;
using RoR2;
using RoR2.Projectile;
using SS2;
using SS2.Equipments;
using SS2.Items;
using UnityEngine;
using Scaling = LookingGlass.ItemStatsNameSpace.ItemStatsDef.ChanceScaling;
using Style = LookingGlass.ItemStatsNameSpace.ItemStatsDef.ValueType;
using Unit = LookingGlass.ItemStatsNameSpace.ItemStatsDef.MeasurementUnits;

namespace RiskyCohesion.LookingGlassStats;

internal static class StarstormStats
{
    private static readonly float BleedoutRechargeTime = 240f;
    private static readonly float BleedoutRechargeReductionPerStack = 0.2f;

    private static readonly float DebuffMissilesPercentChancePerStack = 1f;
    private static readonly float DebuffMissilesDamageCoefficient = 4f;

    private static readonly int ErraticGadgetBouncesPerStack = 2;
    private static readonly float ErraticGadgetChainDistance = 24f;

    private static readonly float HitListMarkInterval = 30f;
    private static readonly float HitListMarkIntervalReductionPerStack = 0.2f;
    private static readonly int HitListItemsPerMarkedKill = 2;
    private static readonly float HitListDamagePerItem = 0.01f;

    private static readonly float InsecticidePoisonPerSecond = 0.15f / 0.25f;
    private static readonly float InsecticideDamageScale = 1.8f;

    private static readonly float ItemOnEliteKillChance = 0.06f;
    private static readonly float ItemOnEliteKillChancePerStack = 0.02f;

    private static readonly float JetBootsProcCoefficient = 1f;

    private static readonly float LowQualitySpeakersProcCoefficient = 1f;

    private static readonly float MaxHpOnKillHealthPerKill = 2f;
    private static readonly float MaxHpOnKillHealthPerKillPerStack = 1f;

    private static readonly float PoisonousGlandDazeDurationPerStack = 2f;

    private static readonly float PortableReactorMovementSpeed = 1f;

    private static readonly float RainbowRootBranches = 3f;
    private static readonly float RainbowRootBarrierPerBranch = 0.25f;
    private static readonly float RainbowRootRechargeTime = 20f;
    private static readonly float RainbowRootRechargeRate = 1f;
    private static readonly float RainbowRootRechargeRatePerStack = 0.5f;
    private static readonly float RainbowRootMaxRechargeRate = 10f;

    private static readonly float RelicOfForceProcCoefficientScale = 0.5f;
    private static readonly float RelicOfForceMaxPenalty = 0.9f;

    private static readonly float RelicOfTerminationCommonWeight = 0.79f;
    private static readonly float RelicOfTerminationUncommonWeightPerStack = 0.2f;
    private static readonly float RelicOfTerminationLegendaryWeightPerStack = 0.01f;
    private static readonly float RelicOfTerminationLegendaryStackExponent = 1.75f;

    private static readonly float ScrapFromChestPercentChance = 33f;
    private static readonly float ScrapFromChestPercentChancePerStack = 11f;

    private static readonly float ShackledLampPrimaryUsesPerShot = 5f;
    private static readonly float ShackledLampDamage = 3f;
    private static readonly float ShackledLampDamagePerStack = 1f;

    private static readonly float ShellPieceInvulnerabilityDuration = 3f;

    private static readonly float ShieldGateShieldPerKill = 2f;
    private static readonly float ShieldGateShieldPerKillPerStack = 1f;

    private static readonly float SnakeEyesPercentCritPerShrine = 3f;
    private static readonly float SnakeEyesPercentCritPerShrinePerStack = 1f;

    private static readonly float StirringSoulChanceScale = 2f;

    private static readonly float ToySoldierCount = 4f;
    private static readonly int ToySoldierFullStatSetting = 10;

    private static readonly float X4LevelRegenDivisor = 5f;

    private static readonly float GreaterWarbannerRechargeRatePerReduction = 2f;

    private static readonly float MidasChestCost = 25f;
    private static readonly float MidasReferenceHealthShare = 0.5f;
    private static readonly float MidasReferenceBaseHealth = 110f;
    private static readonly float MidasReferenceLevelHealth = 33f;

    public static void Install(ConfigFile config)
    {
        var description = new Description("Adds LookingGlass stats for Starstorm 2's items.");
        var stats = StatsGroup.Bind(config, ContentMod.Starstorm.Name, description);

        stats.Item(() => SS2Content.Items.ArmedBackpack, ArmedBackpackStats);
        stats.Item(() => SS2Content.Items.BlastKnuckles, BlastKnucklesStats);
        stats.Item(() => SS2Content.Items.Bleedout, BleedoutStats);
        stats.Item(() => SS2Content.Items.BloodTester, BloodTesterStats);
        stats.Item(() => SS2Content.Items.CoffeeBag, CoffeeBagStats);
        stats.Item(() => SS2Content.Items.CrypticSource, CrypticSourceStats);
        stats.Item(() => SS2Content.Items.DebuffMissiles, DebuffMissilesStats);
        stats.Item(() => SS2Content.Items.DetritiveTrematode, DetritiveTrematodeStats);
        stats.Item(() => SS2Content.Items.Diary, DiaryStats);
        stats.Item(() => SS2Content.Items.DroidHead, DroidHeadStats);
        stats.Item(() => SS2Content.Items.ErraticGadget, ErraticGadgetStats);
        stats.Item(() => SS2Content.Items.FieldAccelerator, FieldAcceleratorStats);
        stats.Item(() => SS2Content.Items.Fork, ForkStats);
        stats.Item(() => SS2Content.Items.GalvanicCore, GalvanicCoreStats);
        stats.Item(() => SS2Content.Items.GreenChocolate, GreenChocolateStats);
        stats.Item(() => SS2Content.Items.GuardingAmulet, GuardingAmuletStats);
        stats.Item(() => SS2Content.Items.HitList, HitListStats);
        stats.Item(() => SS2Content.Items.HuntersSigil, HuntersSigilStats);
        stats.Item(() => SS2Content.Items.IceTool, IceToolStats);
        stats.Item(() => SS2Content.Items.Insecticide, InsecticideStats);
        stats.Item(() => SS2Content.Items.ItemOnEliteKill, ItemOnEliteKillStats);
        stats.Item(() => SS2Content.Items.JetBoots, JetBootsStats);
        stats.Item(() => SS2Content.Items.LightningOnKill, LightningOnKillStats);
        stats.Item(() => SS2Content.Items.LowQualitySpeakers, LowQualitySpeakersStats);
        stats.Item(() => SS2Content.Items.Malice, MaliceStats);
        stats.Item(() => SS2Content.Items.MaxHpOnKill, MaxHpOnKillStats);
        stats.Item(() => SS2Content.Items.MoltenCoin, MoltenCoinStats);
        stats.Item(() => SS2Content.Items.Needles, NeedlesStats);
        stats.Item(() => SS2Content.Items.NkotasHeritage, NkotasHeritageStats);
        stats.Item(() => SS2Content.Items.PoisonousGland, PoisonousGlandStats);
        stats.Item(() => SS2Content.Items.PortableReactor, PortableReactorStats);
        stats.Item(() => SS2Content.Items.PrimalBirthright, PrimalBirthrightStats);
        stats.Item(() => SS2Content.Items.RainbowRoot, RainbowRootStats);
        stats.Item(() => SS2Content.Items.RelicOfForce, RelicOfForceStats);
        stats.Item(() => SS2Content.Items.RelicOfMass, RelicOfMassStats);
        stats.Item(() => SS2Content.Items.RelicOfTermination, RelicOfTerminationStats);
        stats.Item(() => SS2Content.Items.ScrapFromChest, ScrapFromChestStats);
        stats.Item(() => SS2Content.Items.ShackledLamp, ShackledLampStats);
        stats.Item(() => SS2Content.Items.ShellPiece, ShellPieceStats);
        stats.Item(() => SS2Content.Items.ShieldGate, ShieldGateStats);
        stats.Item(() => SS2Content.Items.SnakeEyes, SnakeEyesStats);
        stats.Item(() => SS2Content.Items.StickyOverloader, StickyOverloaderStats);
        stats.Item(() => SS2Content.Items.StirringSoul, StirringSoulStats);
        stats.Item(() => SS2Content.Items.StrangeCan, StrangeCanStats);
        stats.Item(() => SS2Content.Items.SwiftSkateboard, SwiftSkateboardStats);
        stats.Item(() => SS2Content.Items.ToySoldiers, ToySoldiersStats);
        stats.Item(() => SS2Content.Items.UniversalCharger, UniversalChargerStats);
        stats.Item(() => SS2Content.Items.UraniumHorseshoe, UraniumHorseshoeStats);
        stats.Item(() => SS2Content.Items.WatchMetronome, WatchMetronomeStats);
        stats.Item(() => SS2Content.Items.X4, X4Stats);

        stats.Equipment(() => SS2Content.Equipments.BackThruster, BackThrusterStats);
        stats.Equipment(() => SS2Content.Equipments.GreaterWarbanner, GreaterWarbannerStats);
        stats.Equipment(() => SS2Content.Equipments.MIDAS, MidasStats);
    }

    private static float DifficultyScaledCost(int cost) => Run.instance
        ? Run.instance.GetDifficultyScaledCost(cost)
        : cost;

    private static float StageEntryScaledCost(int cost)
    {
        if (!Run.instance)
            return cost;

        var difficulty = Stage.instance ? Stage.instance.entryDifficultyCoefficient : Run.instance.difficultyCoefficient;

        return Run.instance.GetDifficultyScaledCost(cost, difficulty);
    }

    private static ItemStatsDef ArmedBackpackStats()
    {
        var missile = ArmedBackpack.projectilePrefab.GetComponent<ProjectileController>();

        return new StatSheet()
            .Line("Missile Damage: ", Style.Damage, Unit.Percentage)
            .Line("Minimum Chance: ", Style.Damage, Unit.Percentage)
            .Line("Chance Per 1% Base Health Lost: ", Style.Damage, Unit.Percentage)
            .ProcCoefficient()
            .WithLuck((luck, stack, _) =>
            [
                ArmedBackpack.backpackDamageCoeff * stack,
                StatMath.WithLuck(ArmedBackpack.procMinimum / 100f, luck),
                ArmedBackpack.procMult / 100f,
                missile.procCoefficient,
            ]);
    }

    private static ItemStatsDef BlastKnucklesStats() => new StatSheet()
        .Line("Blast Damage: ", Style.Damage, Unit.Percentage)
        .Line("Activation Range: ", Style.Damage, Unit.Meters)
        .Line("Max Charges: ", Style.Utility, Unit.Number)
        .Line("Reload Time: ", Style.Utility, Unit.Seconds)
        .ProcCoefficient()
        .PerStack(stack =>
        [
            StatMath.Linear(BlastKnuckles.damageCoefficient, BlastKnuckles.damageCoefficientPerStack, stack),
            BlastKnuckles.range,
            BlastKnuckles.maxCharges,
            BlastKnuckles.cooldownPerCharge,
            StarstormProcChains.BlastKnucklesBlast.CoefficientInEffect(),
        ]);

    private static ItemStatsDef BleedoutStats() => new StatSheet()
        .Line("Recharge Time: ", Style.Utility, Unit.Seconds)
        .PerStack(stack => [StatMath.ReducedPerStack(BleedoutRechargeTime, BleedoutRechargeReductionPerStack, stack)]);

    private static ItemStatsDef BloodTesterStats() => new StatSheet()
        .Line("Gold Per Heal: ", Style.Gold, Unit.Money)
        .Line("Healing Per Heal: ", Style.Healing, Unit.FlatHealth)
        .Line("Healing Duration: ", Style.Utility, Unit.Seconds)
        .PerBody((body, _) => BloodTesterValues(body));

    private static List<float> BloodTesterValues(CharacterBody body)
    {
        if (!body)
            return null;

        var healing = BloodTester.healthRegen + body.healthComponent.fullHealth * BloodTester.percentHealthRegen / 100f;

        return [DifficultyScaledCost((int)BloodTester.goldRequirement), healing, BloodTester.buffDuration];
    }

    private static ItemStatsDef CoffeeBagStats() => new StatSheet()
        .Line("Bean Chance: ", Style.Damage, Unit.Percentage)
        .Line("Buff Duration: ", Style.Utility, Unit.Seconds)
        .Line("Max Buffs: ", Style.Utility, Unit.Number)
        .Line("Max Attack Speed: ", Style.Damage, Unit.Percentage)
        .Line("Max Movement Speed: ", Style.Utility, Unit.Percentage)
        .Chance(Scaling.DoesNotScale, (luck, stack, procCoefficient) =>
        [
            StatMath.WithLuck(CoffeeBag.procChance * procCoefficient, luck),
            CoffeeBag.buffDuration * stack,
            CoffeeBag.maxStax * stack,
            CoffeeBag.atkSpeedBonus * CoffeeBag.maxStax * stack,
            CoffeeBag.moveSpeedBonus * CoffeeBag.maxStax * stack,
        ]);

    private static ItemStatsDef CrypticSourceStats() => new StatSheet()
        .Line("Burst Damage: ", Style.Damage, Unit.Percentage)
        .Line("Radius: ", Style.Damage, Unit.Meters)
        .Line("Sprint Distance To Charge: ", Style.Utility, Unit.Meters)
        .ProcCoefficient()
        .PerStack(stack =>
        [
            CrypticSource.damageCoefficient * stack,
            StatMath.Linear(CrypticSource.baseRadius, CrypticSource.stackRadius, stack),
            CrypticSource.minimumSprintDistance,
            CrypticSource.procCoefficient,
        ]);

    private static ItemStatsDef DebuffMissilesStats() => new StatSheet()
        .Line("Chance Per Debuff: ", Style.Damage, Unit.Percentage)
        .Line("Total Damage: ", Style.Damage, Unit.Percentage)
        .ProcCoefficient()
        .Chance(Scaling.DoesNotScale, (luck, stack, procCoefficient) =>
        [
            StatMath.WithLuck(StatMath.Linear(DebuffMissiles.percentChancePerDebuff, DebuffMissilesPercentChancePerStack, stack) / 100f * procCoefficient, luck),
            DebuffMissilesDamageCoefficient,
            StarstormProcChains.DebuffMissilesMissiles.CoefficientInEffect(),
        ]);

    private static ItemStatsDef DetritiveTrematodeStats() => new StatSheet()
        .Line("Threshold: ", Style.Health, Unit.PercentHealth)
        .Line("Damage Per Second: ", Style.Damage, Unit.Percentage)
        .PerStack(stack =>
        [
            StatMath.Exponential(DetritiveTrematode.missingHealthPercentage, DetritiveTrematode.missingHealthPercentagePerStack, stack),
            DetritiveTrematode.trematodeDamage * stack,
        ]);

    private static ItemStatsDef DiaryStats() => new StatSheet()
        .Line("Levels Gained: ", Style.Utility, Unit.Number)
        .Line("Charges: ", Style.Utility, Unit.Number)
        .PerStack(stack => [Diary.extraLevels, stack]);

    private static ItemStatsDef DroidHeadStats() => new StatSheet()
        .Line("Drone Base Damage: ", Style.Damage, Unit.Percentage)
        .Line("Drone Duration: ", Style.Utility, Unit.Seconds)
        .PerStack(stack =>
        [
            DroidHead.baseDamage * stack,
            StatMath.Linear(DroidHead.baseLifeTime, DroidHead.stackLifeTime, stack),
        ]);

    private static ItemStatsDef ErraticGadgetStats() => new StatSheet()
        .Line("Proc Chance: ", Style.Damage, Unit.Percentage)
        .Line("Lightning Damage: ", Style.Damage, Unit.Percentage)
        .Line("Max Targets: ", Style.Damage, Unit.Number)
        .Line("Chain Distance: ", Style.Damage, Unit.Meters)
        .Line("Repeated Lightning Damage: ", Style.Damage, Unit.Percentage)
        .ProcCoefficient()
        .Chance(Scaling.DoesNotScale, (luck, stack, procCoefficient) =>
        [
            StatMath.WithLuck(ErraticGadget.procChance * procCoefficient, luck),
            ErraticGadget.damageCoefficient,
            1 + StatMath.Linear(ErraticGadget.bounceTargets, ErraticGadgetBouncesPerStack, stack),
            ErraticGadgetChainDistance,
            ErraticGadget.repeatDamageMultiplier,
            StarstormProcChains.ErraticGadgetLightning.CoefficientInEffect(),
        ]);

    private static ItemStatsDef FieldAcceleratorStats() => new StatSheet()
        .Line("Teleporter Radius Bonus: ", Style.Utility, Unit.Percentage)
        .Line("Charge Per Kill: ", Style.Utility, Unit.Percentage)
        .PerStack(stack => [FieldAccelerator.radiusPerStack * stack, FieldAccelerator.chargePerKill]);

    private static ItemStatsDef ForkStats() => new StatSheet()
        .Line("Bonus Damage: ", Style.Damage, Unit.Percentage)
        .PerStack(stack => [Fork.percentDamageBonus / 100f * stack]);

    private static ItemStatsDef GalvanicCoreStats() => new StatSheet()
        .Line("Stun Chance: ", Style.Utility, Unit.Percentage)
        .Line("Aura Radius: ", Style.Utility, Unit.Meters)
        .Line("Max Health Reduction: ", Style.Utility, Unit.Percentage)
        .Line("Movement Speed Reduction: ", Style.Utility, Unit.Percentage)
        .Chance(Scaling.Hyperbolic, (luck, stack, procCoefficient) =>
        [
            StatMath.WithLuck(StatMath.Hyperbolic(GalvanicCore.stunChance, GalvanicCore.stunChanceStacking, stack) * procCoefficient, luck),
            StatMath.Linear(GalvanicCore.baseAuraRadius, GalvanicCore.scalingAuraRadius, stack),
            1f - 1f / (1f + GalvanicCore.healthPenaltyCalculated),
            1f - 1f / (1f + GalvanicCore.speedPenalty),
        ]);

    private static ItemStatsDef GreenChocolateStats() => new StatSheet()
        .Line("Damage Threshold: ", Style.Health, Unit.PercentHealth)
        .Line("Excess Damage Reduction: ", Style.Armor, Unit.Percentage)
        .Line("Buff Duration: ", Style.Damage, Unit.Seconds)
        .Line("Buff Damage: ", Style.Damage, Unit.Percentage)
        .Line("Buff Crit Chance: ", Style.Damage, Unit.Percentage)
        .PerStack(stack =>
        [
            GreenChocolate.damageThreshold,
            GreenChocolate.damageReduction,
            StatMath.Linear(GreenChocolate.baseDuration, GreenChocolate.stackDuration, stack),
            GreenChocolate.buffDamage,
            GreenChocolate.buffCrit / 100f,
        ]);

    private static ItemStatsDef GuardingAmuletStats() => new StatSheet()
        .Line("Damage Reduction From Behind: ", Style.Armor, Unit.Percentage)
        .PerStack(stack => [StatMath.Hyperbolic(GuardingAmulet.damageReduction, GuardingAmulet.damageReduction, stack)]);

    private static ItemStatsDef HitListStats() => new StatSheet()
        .Line("Mark Interval: ", Style.Utility, Unit.Seconds)
        .Line("Damage Per Marked Kill: ", Style.Damage, Unit.Percentage)
        .Line("Current Damage Bonus: ", Style.Damage, Unit.Percentage)
        .PerMaster(HitListValues);

    private static List<float> HitListValues(CharacterMaster master, int stack)
    {
        if (!master)
            return null;

        return
        [
            StatMath.ReducedPerStack(HitListMarkInterval, HitListMarkIntervalReductionPerStack, stack),
            HitListItemsPerMarkedKill * HitListDamagePerItem,
            Holder.ItemCount(master, SS2Content.Items.StackHitList) * HitListDamagePerItem,
        ];
    }

    private static ItemStatsDef HuntersSigilStats() => new StatSheet()
        .Line("Bonus Armor: ", Style.Armor, Unit.Number)
        .Line("Bonus Damage: ", Style.Damage, Unit.Percentage)
        .Line("Radius: ", Style.Utility, Unit.Meters)
        .PerStack(stack =>
        [
            StatMath.Linear(HuntersSigil.baseArmor, HuntersSigil.stackArmor, stack),
            StatMath.Linear(HuntersSigil.baseDamage, HuntersSigil.stackDamage, stack),
            HuntersSigil.radius,
        ]);

    private static ItemStatsDef IceToolStats() => new StatSheet()
        .Line("Extra Wall Jumps: ", Style.Utility, Unit.Number)
        .PerStack(stack => [stack]);

    private static ItemStatsDef InsecticideStats() => new StatSheet()
        .Line("Poison Chance: ", Style.Damage, Unit.Percentage)
        .Line("Poison Damage: ", Style.Damage, Unit.Percentage)
        .Line("Poison Duration: ", Style.Damage, Unit.Seconds)
        .Chance(Scaling.DoesNotScale, (luck, stack, procCoefficient) =>
        [
            StatMath.WithLuck(Insecticide.chance * procCoefficient, luck),
            Insecticide.damageCoeff / InsecticideDamageScale * InsecticidePoisonPerSecond * Insecticide.duration * stack,
            Insecticide.duration,
        ]);

    private static ItemStatsDef ItemOnEliteKillStats() => new StatSheet()
        .Line("Elite Drop Chance: ", Style.Utility, Unit.Percentage)
        .PerStack(stack => [StatMath.Linear(ItemOnEliteKillChance, ItemOnEliteKillChancePerStack, stack)]);

    private static ItemStatsDef JetBootsStats() => new StatSheet()
        .Line("Explosion Damage: ", Style.Damage, Unit.Percentage)
        .Line("Explosion Radius: ", Style.Damage, Unit.Meters)
        .Line("Recharge Time: ", Style.Utility, Unit.Seconds)
        .ProcCoefficient()
        .PerStack(stack =>
        [
            StatMath.Linear(JetBoots.baseDamage, JetBoots.stackDamage, stack),
            StatMath.Linear(JetBoots.baseRadius, JetBoots.stackRadius, stack),
            JetBoots.jumpCooldown,
            JetBootsProcCoefficient,
        ]);

    private static ItemStatsDef LightningOnKillStats() => new StatSheet()
        .Line("Lightning Damage: ", Style.Damage, Unit.Percentage)
        .Line("Max Targets: ", Style.Damage, Unit.Number)
        .Line("Chain Distance: ", Style.Damage, Unit.Meters)
        .ProcCoefficient()
        .PerStack(stack =>
        [
            LightningOnKill.damageCoeff,
            StatMath.Linear(LightningOnKill.bounceBase, LightningOnKill.bounceStack, stack),
            StatMath.Linear(LightningOnKill.radiusBase, LightningOnKill.radiusPerStack, stack),
            LightningOnKill.procCo,
        ]);

    private static ItemStatsDef LowQualitySpeakersStats() => new StatSheet()
        .Line("Chance At Full Health: ", Style.Utility, Unit.Percentage)
        .Line("Proc Cooldown: ", Style.Utility, Unit.Seconds)
        .Line("Radius: ", Style.Utility, Unit.Meters)
        .ProcCoefficient()
        .WithLuck((luck, stack, _) =>
        [
            StatMath.WithLuck(LowQualitySpeakers.baseProcChance, luck),
            LowQualitySpeakers.cooldown,
            LowQualitySpeakers.baseRadius + stack * (LowQualitySpeakers.radiusPerStack - 1f),
            LowQualitySpeakersProcCoefficient,
        ]);

    private static ItemStatsDef MaliceStats() => new StatSheet()
        .Line("Total Damage: ", Style.Damage, Unit.Percentage)
        .Line("Max Targets: ", Style.Damage, Unit.Number)
        .Line("Radius: ", Style.Damage, Unit.Meters)
        .ProcCoefficient()
        .PerStack(stack =>
        [
            Malice.damageCoeff,
            Malice.bounceStack * stack,
            StatMath.Linear(Malice.radiusBase, Malice.radiusPerStack, stack),
            Malice.procCo,
        ]);

    private static ItemStatsDef MaxHpOnKillStats() => new StatSheet()
        .Line("Health Per Kill: ", Style.Healing, Unit.FlatHealth)
        .Line("Current Bonus: ", Style.Healing, Unit.FlatHealth)
        .PerMaster((master, stack) =>
        [
            StatMath.Linear(MaxHpOnKillHealthPerKill, MaxHpOnKillHealthPerKillPerStack, stack),
            Holder.ItemCount(master, SS2Content.Items.StackMaxHpOnKill),
        ]);

    private static ItemStatsDef MoltenCoinStats() => new StatSheet()
        .Line("Ignite Chance: ", Style.Damage, Unit.Percentage)
        .Line("Total Burn Damage: ", Style.Damage, Unit.Percentage)
        .Line("Burn Duration: ", Style.Damage, Unit.Seconds)
        .Line("Gold Per Ignite: ", Style.Gold, Unit.Money)
        .Chance(Scaling.DoesNotScale, (luck, _, procCoefficient) =>
        [
            StatMath.WithLuck(MoltenCoin.procChance / 100f * procCoefficient, luck),
            MoltenCoin.damageCoeff,
            MoltenCoin.burnDuration,
            StageEntryScaledCost(MoltenCoin.coinGain),
        ]);

    private static ItemStatsDef NeedlesStats() => new StatSheet()
        .Line("Strike Chance On Crit: ", Style.Damage, Unit.Percentage)
        .Line("Crit Chance: ", Style.Damage, Unit.Percentage)
        .Line("Strikes: ", Style.Damage, Unit.Number)
        .Line("Damage Per Strike: ", Style.Damage, Unit.Percentage)
        .ProcCoefficient()
        .Chance(Scaling.DoesNotScale, (luck, stack, procCoefficient) =>
        [
            StatMath.WithLuck(Needles.needleStrikeChance / 100f * procCoefficient, luck),
            Needles.critChancePerStack / 100f * stack,
            Needles.needleStrikeCount,
            Needles.needleDamageCoefficient * stack,
            StarstormProcChains.NeedlesStrikes.CoefficientInEffect(),
        ]);

    private static ItemStatsDef NkotasHeritageStats() => new StatSheet()
        .Line("Items Per Level Up: ", Style.Utility, Unit.Number)
        .PerStack(stack => [NkotasHeritage.itemsPerStack * stack]);

    private static ItemStatsDef PoisonousGlandStats() => new StatSheet()
        .Line("Daze Duration: ", Style.Utility, Unit.Seconds)
        .Line("Miss Chance: ", Style.Utility, Unit.Percentage)
        .PerStack(stack =>
        [
            PoisonousGlandDazeDurationPerStack * stack,
            Util.ConvertAmplificationPercentageIntoReductionPercentage(PoisonousGland.missChance) / 100f,
        ]);

    private static ItemStatsDef PortableReactorStats() => new StatSheet()
        .Line("Invulnerability Duration: ", Style.Utility, Unit.Seconds)
        .Line("Movement Speed: ", Style.Utility, Unit.Percentage)
        .PerStack(stack =>
        [
            StatMath.Linear(PortableReactor.invulnTime, PortableReactor.stackingInvuln, stack),
            PortableReactorMovementSpeed,
        ]);

    private static ItemStatsDef PrimalBirthrightStats() => new StatSheet()
        .Line("Legendary Chests: ", Style.Utility, Unit.Number)
        .Line("Time Limit: ", Style.Death, Unit.Seconds)
        .Line("Chest Price Multiplier: ", Style.Gold, Unit.Percentage)
        .PerStack(stack =>
        [
            stack,
            StatMath.Linear(PrimalBirthright.birthrightCompletionTime, PrimalBirthright.birthrightCompletionTimeStacking, stack),
            PrimalBirthright.birthrightPriceModifier,
        ]);

    private static ItemStatsDef RainbowRootStats() => new StatSheet()
        .Line("Branches: ", Style.Utility, Unit.Number)
        .Line("Barrier Per Branch: ", Style.Healing, Unit.PercentHealth)
        .Line("Recharge Time: ", Style.Utility, Unit.Seconds)
        .PerStack(stack =>
        [
            RainbowRootBranches,
            RainbowRootBarrierPerBranch,
            RainbowRootRechargeTime / MSUtil.InverseHyperbolicScaling(RainbowRootRechargeRate, RainbowRootRechargeRatePerStack, RainbowRootMaxRechargeRate, stack),
        ]);

    private static ItemStatsDef RelicOfForceStats() => new StatSheet()
        .Line("Extra Hits: ", Style.Damage, Unit.Number)
        .Line("Damage Increase Per Hit: ", Style.Damage, Unit.Percentage)
        .Line("Max Extra Hit Damage: ", Style.Damage, Unit.Percentage)
        .Line("Attack Speed Reduction: ", Style.Death, Unit.Percentage)
        .Line("Cooldown Increase: ", Style.Death, Unit.Percentage)
        .ProcCoefficient()
        .PerStack(stack =>
        [
            stack,
            RelicOfForce.hitIncrease,
            RelicOfForce.hitMax,
            RelicOfForcePenalty(stack),
            RelicOfForcePenalty(stack),
            RelicOfForceProcCoefficientScale,
        ]);

    private static float RelicOfForcePenalty(int stack) => MSUtil.InverseHyperbolicScaling(RelicOfForce.forcePenalty, RelicOfForce.forcePenalty, RelicOfForceMaxPenalty, stack);

    private static ItemStatsDef RelicOfMassStats() => new StatSheet()
        .Line("Health Bonus: ", Style.Healing, Unit.Percentage)
        .Line("Acceleration Multiplier: ", Style.Death, Unit.Percentage)
        .PerStack(stack => [RelicOfMass.healthIncrease * stack, 1f / (RelicOfMass.acclMult * stack)]);

    private static ItemStatsDef RelicOfTerminationStats() => new StatSheet()
        .Line("Time Limit: ", Style.Utility, Unit.Seconds)
        .Line("Common Chance: ", Style.Sub, Unit.Percentage)
        .Line("Uncommon Chance: ", Style.Healing, Unit.Percentage)
        .Line("Legendary Chance: ", Style.Health, Unit.Percentage)
        .Line("Marked Health Bonus: ", Style.Death, Unit.Percentage)
        .Line("Marked Damage Bonus: ", Style.Death, Unit.Percentage)
        .PerStack(RelicOfTerminationValues);

    private static List<float> RelicOfTerminationValues(int stack)
    {
        var common = RelicOfTerminationCommonWeight;
        var uncommon = RelicOfTerminationUncommonWeightPerStack * stack;
        var legendary = RelicOfTerminationLegendaryWeightPerStack * Mathf.Pow(stack, RelicOfTerminationLegendaryStackExponent);
        var total = common + uncommon + legendary;

        return
        [
            StatMath.ReducedPerStack(RelicOfTermination.maxTime, RelicOfTermination.timeReduction, stack),
            common / total,
            uncommon / total,
            legendary / total,
            RelicOfTermination.hpMult,
            RelicOfTermination.damageMult,
        ];
    }

    private static ItemStatsDef ScrapFromChestStats() => new StatSheet()
        .Line("Scrap Chance: ", Style.Utility, Unit.Percentage)
        .PerStack(stack => [Mathf.Min(StatMath.Linear(ScrapFromChestPercentChance, ScrapFromChestPercentChancePerStack, stack) / 100f, 1f)]);

    private static ItemStatsDef ShackledLampStats()
    {
        var projectile = ShackledLamp.projectilePrefab;
        var explosion = projectile.GetComponent<ProjectileImpactExplosion>();
        var procCoefficient = projectile.GetComponent<ProjectileController>().procCoefficient * explosion.blastProcCoefficient;

        return new StatSheet()
            .Line("Primary Uses Per Shot: ", Style.Damage, Unit.Number)
            .Line("Projectile Damage: ", Style.Damage, Unit.Percentage)
            .ProcCoefficient()
            .PerStack(stack =>
            [
                ShackledLampPrimaryUsesPerShot,
                StatMath.Linear(ShackledLampDamage, ShackledLampDamagePerStack, stack) * explosion.blastDamageCoefficient,
                procCoefficient,
            ]);
    }

    private static ItemStatsDef ShellPieceStats() => new StatSheet()
        .Line("Charges: ", Style.Utility, Unit.Number)
        .Line("Invulnerability Duration: ", Style.Utility, Unit.Seconds)
        .PerStack(stack => [stack, ShellPieceInvulnerabilityDuration]);

    private static ItemStatsDef ShieldGateStats() => new StatSheet()
        .Line("Shield Per Kill: ", Style.Utility, Unit.FlatHealth)
        .Line("Current Bonus: ", Style.Utility, Unit.FlatHealth)
        .PerMaster((master, stack) =>
        [
            StatMath.Linear(ShieldGateShieldPerKill, ShieldGateShieldPerKillPerStack, stack),
            Holder.ItemCount(master, SS2Content.Items.StackShieldGate),
        ]);

    private static ItemStatsDef SnakeEyesStats() => new StatSheet()
        .Line("Crit Chance Per Shrine: ", Style.Damage, Unit.Percentage)
        .Line("Current Bonus: ", Style.Damage, Unit.Percentage)
        .PerMaster((master, stack) =>
        [
            StatMath.Linear(SnakeEyesPercentCritPerShrine, SnakeEyesPercentCritPerShrinePerStack, stack) / 100f,
            Holder.ItemCount(master, SS2Content.Items.StackSnakeEyes) / 100f,
        ]);

    private static ItemStatsDef StickyOverloaderStats() => new StatSheet()
        .Line("Bomb Chance: ", Style.Damage, Unit.Percentage)
        .Line("Damage Per Charge: ", Style.Damage, Unit.Percentage)
        .Line("Radius Per Charge: ", Style.Damage, Unit.Meters)
        .Line("Max Charges: ", Style.Damage, Unit.Number)
        .ProcCoefficient()
        .Chance(Scaling.DoesNotScale, (luck, stack, procCoefficient) =>
        [
            StatMath.WithLuck(StickyOverloader.procChance / 100f * procCoefficient, luck),
            StatMath.Linear(StickyOverloader.damageCoefficient, StickyOverloader.damageCoefficientPerStack, stack),
            StickyOverloader.blastRadius,
            StatMath.Linear(StickyOverloader.maxStacks, StickyOverloader.maxStacksPerStack, stack),
            StarstormProcChains.StickyOverloaderDetonation.CoefficientInEffect(),
        ]);

    private static ItemStatsDef StirringSoulStats() => new StatSheet()
        .Line("Chance Per Failed Roll: ", Style.Utility, Unit.Percentage)
        .Line("Max Chance: ", Style.Utility, Unit.Percentage)
        .PerStack(_ => [StirringSoul.initChance * StirringSoulChanceScale, StirringSoul.maxChance * StirringSoulChanceScale]);

    private static ItemStatsDef StrangeCanStats() => new StatSheet()
        .Line("Intoxicate Chance: ", Style.Damage, Unit.Percentage)
        .Line("Damage Per Second: ", Style.Damage, Unit.Percentage)
        .Line("Max Stacks: ", Style.Damage, Unit.Number)
        .Line("Duration: ", Style.Damage, Unit.Seconds)
        .Line("Healing Per Stack: ", Style.Healing, Unit.FlatHealth)
        .Chance(Scaling.DoesNotScale, (luck, stack, procCoefficient) =>
        [
            StatMath.WithLuck(StatMath.Linear(StrangeCan.procChance, StrangeCan.procChancePerStack, stack) / 100f * procCoefficient, luck),
            StrangeCan.damageCoefficient,
            StrangeCan.maxStacks,
            StrangeCan.buffDuration,
            StatMath.Linear(StrangeCan.healAmount, StrangeCan.healAmountPerStack, stack),
        ]);

    private static ItemStatsDef SwiftSkateboardStats() => new StatSheet()
        .Line("Speed Per Buff: ", Style.Utility, Unit.Percentage)
        .Line("Max Buffs: ", Style.Utility, Unit.Number)
        .Line("Max Movement Speed: ", Style.Utility, Unit.Percentage)
        .Line("Buff Duration: ", Style.Utility, Unit.Seconds)
        .PerStack(SwiftSkateboardValues);

    private static List<float> SwiftSkateboardValues(int stack)
    {
        var speedPerBuff = StatMath.Linear(SwiftSkateboard.moveSpeedBonus, SwiftSkateboard.moveSpeedBonusPerStack, stack);
        var maxBuffs = StatMath.Linear(SwiftSkateboard.maxStacks, SwiftSkateboard.maxStacksPerStack, stack);

        return [speedPerBuff, maxBuffs, speedPerBuff * maxBuffs, SwiftSkateboard.buffDuration];
    }

    private static ItemStatsDef ToySoldiersStats() => new StatSheet()
        .Line("Soldiers: ", Style.Damage, Unit.Number)
        .Line("Soldier Damage: ", Style.Damage, Unit.Percentage)
        .Line("Soldier Health: ", Style.Healing, Unit.Percentage)
        .PerStack(stack =>
        [
            ToySoldierCount,
            ToySoldierStatMultiplier(ToySoldiers.stackDamage, ToySoldiers.baseDamage, stack),
            ToySoldierStatMultiplier(ToySoldiers.stackHealth, ToySoldiers.baseHealth, stack),
        ]);

    private static float ToySoldierStatMultiplier(int boostsPerStack, int baseStatSetting, int stack)
        => StatMath.BoostMultiplier((boostsPerStack + Mathf.Max(baseStatSetting - ToySoldierFullStatSetting, 0)) * stack);

    private static ItemStatsDef UniversalChargerStats() => new StatSheet()
        .Line("Recharge Time: ", Style.Utility, Unit.Seconds)
        .PerStack(stack => [UniversalChargerRechargeTime(stack)]);

    private static float UniversalChargerRechargeTime(int stack)
    {
        var reduction = Util.ConvertAmplificationPercentageIntoReductionPercentage(UniversalCharger.cooldownReductionPerStack * (stack - 1)) / 100f;

        return UniversalCharger.baseCooldown * (1f - reduction);
    }

    private static ItemStatsDef UraniumHorseshoeStats() => new StatSheet()
        .Line("Movement Speed: ", Style.Utility, Unit.Percentage)
        .Line("Jump Power: ", Style.Utility, Unit.Percentage)
        .PerStack(stack => [UraniumHoreshoe.movespeed * stack, UraniumHoreshoe.jumpBoost * stack]);

    private static ItemStatsDef WatchMetronomeStats() => new StatSheet()
        .Line("Max Sprint Speed Bonus: ", Style.Utility, Unit.Percentage)
        .Line("Charge Time: ", Style.Utility, Unit.Seconds)
        .Line("Decay Time: ", Style.Utility, Unit.Seconds)
        .PerStack(stack =>
        [
            WatchMetronome.maxMovementSpeed,
            WatchMetronome.maxBuffStacks / WatchMetronome.gainPerSecond,
            StatMath.Linear(WatchMetronome.maxDuration, WatchMetronome.maxDurationPerStack, stack),
        ]);

    private static ItemStatsDef X4Stats() => new StatSheet()
        .Line("Secondary Cooldown Reduction: ", Style.Utility, Unit.Percentage)
        .Line("Regen Per Use: ", Style.Healing, Unit.FlatHealing)
        .Line("Regen Duration: ", Style.Healing, Unit.Seconds)
        .PerBody((body, stack) =>
        [
            StatMath.Exponential(X4.cdReduction, X4.cdReduction, stack),
            X4RegenPerUse(body, stack),
            X4.regenDuration,
        ]);

    private static float X4RegenPerUse(CharacterBody body, int stack)
    {
        var regen = StatMath.Linear(X4.baseRegenBoost, X4.stackRegenBoost, stack);
        var level = body ? body.level : 1f;

        return (regen + regen / X4LevelRegenDivisor * level) * Holder.RegenMultiplier(body);
    }

    private static ItemStatsDef BackThrusterStats() => new StatSheet()
        .Line("Time To Max Speed: ", Style.Utility, Unit.Seconds)
        .PerStack(_ => [BackThruster.accel]);

    private static ItemStatsDef GreaterWarbannerStats()
    {
        var ward = SS2Assets.LoadAsset<GameObject>("GreaterWarbannerWard", SS2Bundle.Equipments).GetComponent<BuffWard>();

        return new StatSheet()
            .Line("Radius: ", Style.Utility, Unit.Meters)
            .Line("Skill Cooldown Reduction: ", Style.Utility, Unit.Percentage)
            .PerStack(_ =>
            [
                ward.radius,
                1f - 1f / (1f + GreaterWarbannerRechargeRatePerReduction * GreaterWarbanner.cooldownReduction),
            ]);
    }

    private static ItemStatsDef MidasStats() => new StatSheet()
        .Line("Health Cost: ", Style.Health, Unit.FlatHealth)
        .Line("Gold Gained: ", Style.Gold, Unit.Money)
        .PerStack(_ => MidasValues(LocalUserManager.GetFirstLocalUser()?.cachedBody));

    private static List<float> MidasValues(CharacterBody body)
    {
        if (!body || !Run.instance)
            return null;

        var healthCost = body.healthComponent.health * MIDAS.healthPercentage;
        var referenceHealth = MidasReferenceHealthShare * (MidasReferenceBaseHealth + MidasReferenceLevelHealth * (body.level - 1f));
        var chestShare = healthCost / referenceHealth;
        var chestGold = Run.instance.GetDifficultyScaledCost((int)(MidasChestCost * chestShare));

        return [healthCost, Mathf.CeilToInt(Mathf.Max(chestGold, healthCost))];
    }
}
