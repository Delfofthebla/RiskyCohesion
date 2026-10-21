using System;
using System.Linq;
using AncientScepter;
using BepInEx.Configuration;
using RiskyCohesion.Sources;
using RiskyMod.Survivors.Mage;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;
using OrigOnHitEnemy = On.RoR2.GlobalEventManager.orig_OnHitEnemy;

namespace RiskyCohesion.Survivors;

internal static class AncientScepterSkills
{
    public static void Install(ConfigFile config)
    {
        MatchDeathBlossom(config);
        MatchRabauld(config);
        MatchLoaderSprint(config, "Megaton Punch", "Charged Gauntlet", "RoR2/Base/Loader/ChargeFist.asset", Scepter<LoaderChargeFist2>());
        MatchLoaderSprint(config, "Thundercrash", "Thunder Gauntlet", "RoR2/Base/Loader/ChargeZapFist.asset", Scepter<LoaderChargeZapFist2>());
        MatchThundercrashRange(config);
        MatchDragonsBreath(config);
        MatchForfeitedSuppress(config);
        MatchLilithStrike(config);
        MatchReap(config);
        SkipRenegadeRicochet(config);
    }

    private static void MatchDeathBlossom(ConfigFile config)
    {
        var description = new Description("Death Blossom keeps the cooldown, hold-to-fire and description RiskyMod gives Suppressive Fire.")
            .Requires(RiskyModSource.SuppressiveFireSetting);

        var deathBlossom = Bind(config, RiskyModSource.ChangesSuppressiveFire, "Death Blossom", description);
        if (!deathBlossom.SourceIsOn)
            return;

        var suppressiveFire = LoadSkill("RoR2/Base/Commando/CommandoBodyBarrage.asset");
        var scepter = Scepter<CommandoBarrage2>();

        deathBlossom.MatchFields(suppressiveFire, scepter.myDef, nameof(SkillDef.baseRechargeInterval), nameof(SkillDef.interruptPriority), nameof(SkillDef.mustKeyPress));
        MatchDescription(deathBlossom, scepter, suppressiveFire);
    }

    private static void MatchRabauld(ConfigFile config)
    {
        var description = new Description("Rabauld keeps the cooldown RiskyMod gives Ballista.")
            .Requires(RiskyModSource.BallistaSetting);

        var rabauld = Bind(config, RiskyModSource.ChangesBallista, "Rabauld", description);
        if (!rabauld.SourceIsOn)
            return;

        var ballista = LoadSkill("RoR2/Base/Huntress/AimArrowSnipe.asset");

        rabauld.MatchFields(ballista, Scepter<HuntressBallista2>().myDef, nameof(SkillDef.baseRechargeInterval), nameof(SkillDef.beginSkillCooldownOnSkillEnd));
    }

    private static void MatchLoaderSprint(ConfigFile config, string key, string baseSkillName, string baseSkillAsset, ScepterSkill scepter)
    {
        var description = new Description($"{key} keeps sprinting, like RiskyTweaks' {baseSkillName}.")
            .Requires(RiskyTweaksSource.LoaderSprintSetting);

        var keepSprinting = Bind(config, RiskyTweaksSource.LoaderSkillsKeepSprint, key, description);
        if (!keepSprinting.SourceIsOn)
            return;

        var baseSkill = LoadSkill(baseSkillAsset);

        keepSprinting.MatchFields(baseSkill, scepter.myDef, nameof(SkillDef.cancelSprintingOnActivation));
    }

    private static void MatchThundercrashRange(ConfigFile config)
    {
        var description = new Description("Thundercrash's lightning gets the extra range RiskyMod gives Thunder Gauntlet's.")
            .Requires(RiskyModSource.ThunderGauntletSetting);

        var thundercrashRange = Bind(config, RiskyModSource.ChangesThunderGauntlet, "Thundercrash Range", description);
        if (!thundercrashRange.SourceIsOn)
            return;

        var thunderGauntletCone = LoadComponent<ProjectileProximityBeamController>("RoR2/Base/Loader/LoaderZapCone.prefab");
        var thundercrashCone = LoaderChargeZapFist2.projReplacer.GetComponent<ProjectileProximityBeamController>();

        thundercrashRange.MatchFields(thunderGauntletCone, thundercrashCone, nameof(ProjectileProximityBeamController.attackRange));
    }

    private static void MatchDragonsBreath(ConfigFile config)
    {
        var description = new Description("Dragon's Breath follows RiskyMod's Flamethrower - Sprint Cancel.")
            .Requires(RiskyModSource.ArtificerSetting);

        var dragonsBreath = Bind(config, RiskyModSource.ChangesArtificer, "Dragons Breath", description);
        if (!dragonsBreath.SourceIsOn)
            return;

        var flamethrower = LoadSkill("RoR2/Base/Mage/MageBodyFlamethrower.asset");
        var scepter = Scepter<ArtificerFlamethrower2>().myDef;
        EventHandler matchSprintCancel = (_, _) => scepter.canceledFromSprinting = flamethrower.canceledFromSprinting;

        dragonsBreath.MatchFields(flamethrower, scepter, nameof(SkillDef.canceledFromSprinting));
        dragonsBreath.Subscribe(() => MageCore.flamethrowerSprintCancel.SettingChanged += matchSprintCancel, () => MageCore.flamethrowerSprintCancel.SettingChanged -= matchSprintCancel);
    }

    private static void MatchForfeitedSuppress(ConfigFile config)
    {
        var description = new Description("Forfeited Suppress refills instantly, like RiskyTweaks' Corrupted Suppress.")
            .Requires(RiskyTweaksSource.CorruptedCrushSetting);

        var forfeitedSuppress = Bind(config, RiskyTweaksSource.BuffsCorruptedCrush, "Forfeited Suppress", description);
        if (!forfeitedSuppress.SourceIsOn)
            return;

        var corruptedSuppress = LoadSkill("RoR2/DLC1/VoidSurvivor/CrushHealth.asset");

        forfeitedSuppress.MatchFields(corruptedSuppress, VoidFiendCrush.myCtxDef, nameof(SkillDef.baseRechargeInterval), nameof(SkillDef.rechargeStock));
    }

    private static void MatchLilithStrike(ConfigFile config)
    {
        var description = new Description("Lilith Strike's blast keeps the falloff, proc coefficient and friendly fire RiskyMod gives Diablo Strike's.")
            .Requires(RiskyModSource.DiabloStrikeSetting);

        var lilithStrike = Bind(config, RiskyModSource.ChangesDiabloStrike, "Lilith Strike", description);
        if (!lilithStrike.SourceIsOn)
            return;

        var diabloBlast = LoadComponent<ProjectileImpactExplosion>("RoR2/Base/Captain/CaptainAirstrikeAltProjectile.prefab");
        var lilithBlast = CaptainAirstrikeAlt2.airstrikePrefab.GetComponent<ProjectileImpactExplosion>();

        lilithStrike.MatchFields(
            diabloBlast,
            lilithBlast,
            nameof(ProjectileExplosion.falloffModel),
            nameof(ProjectileExplosion.blastProcCoefficient),
            nameof(ProjectileExplosion.blastAttackerFiltering));
    }

    private static void MatchReap(ConfigFile config)
    {
        var description = new Description("COMMAND: REAP keeps the fruit pickup range and description RiskyMod gives DIRECTIVE: Harvest.")
            .Requires(RiskyModSource.HarvestSetting);

        var reap = Bind(config, RiskyModSource.ChangesHarvest, "COMMAND: REAP", description);
        if (!reap.SourceIsOn)
            return;

        var harvest = LoadSkill("RoR2/Base/Treebot/TreebotBodyFireFruitSeed.asset");
        var harvestFruit = FruitPickupRange(LoadPrefab("RoR2/Base/Treebot/TreebotFruitPack.prefab"));
        var reapFruit = FruitPickupRange(TreebotFireFruitSeed2.ScepterTreebotFruitPackPrefab);

        reap.Override(() => reapFruit.localScale, scale => reapFruit.localScale = scale, _ => harvestFruit.localScale);
        MatchDescription(reap, Scepter<TreebotFireFruitSeed2>(), harvest);
    }

    private static void SkipRenegadeRicochet(ConfigFile config)
    {
        var description = new Description("Renegade's ricochet no longer triggers on RiskyMod's reworked specials, which have their own scepter upgrades.")
            .Requires(RiskyModSource.BanditSpecialReworkSetting);

        var skipRicochet = Bind(config, RiskyModSource.ReworksBanditSpecials, "Renegade Ricochet", description);

        skipRicochet.Hook(typeof(Bandit2SkullRevolver2), nameof(Bandit2SkullRevolver2.GlobalEventManager_OnHitEnemy), SkipRicochet);
    }

    private static void SkipRicochet(
        Action<Bandit2SkullRevolver2, OrigOnHitEnemy, GlobalEventManager, DamageInfo, GameObject> orig,
        Bandit2SkullRevolver2 renegade,
        OrigOnHitEnemy next,
        GlobalEventManager manager,
        DamageInfo damageInfo,
        GameObject victim) => next(manager, damageInfo, victim);

    private static Rule Bind(ConfigFile config, bool sourceIsOn, string key, Description description)
        => new(config.BindWhen(sourceIsOn, ContentMod.AncientScepter.SurvivorsSection, key, true, description));

    private static void MatchDescription(Rule rule, ScepterSkill scepter, SkillDef baseSkill)
        => rule.Override(() => scepter.oldDescToken, token => scepter.oldDescToken = token, _ => baseSkill.skillDescriptionToken);

    private static T Scepter<T>() where T : ScepterSkill => ItemBase<AncientScepterItem>.instance.skills.OfType<T>().First();

    private static SkillDef LoadSkill(string key) => Addressables.LoadAssetAsync<SkillDef>(key).WaitForCompletion();

    private static GameObject LoadPrefab(string key) => Addressables.LoadAssetAsync<GameObject>(key).WaitForCompletion();

    private static T LoadComponent<T>(string prefabKey) => LoadPrefab(prefabKey).GetComponent<T>();

    private static Transform FruitPickupRange(GameObject fruitPack) => fruitPack.GetComponentInChildren<GravitatePickup>().transform;
}
