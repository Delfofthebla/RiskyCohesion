using System;
using BepInEx.Configuration;
using ChefOvercooked;
using R2API;
using RiskyCohesion.Sources;
using RoR2;
using UnityEngine;
using ChefConfig = ChefOvercooked.PluginConfig;

namespace RiskyCohesion.Survivors;

internal static class ChefOvercookedCook
{
    private static readonly SharedHook<Rule> _hitTracking = new(typeof(GlobalEventManager), nameof(GlobalEventManager.ServerDamageDealt), TrackResolvingHit);

    private static DamageReport _resolvingHit;

    public static void Install(ConfigFile config)
    {
        var section = ContentMod.ChefOvercooked.SurvivorsSection;

        var executeDescription = new Description("Cook's hits execute enemies below its Execute Threshold again.")
            .Requires(RiskyModSource.FreezeExecutesBossesSetting);

        var execute = new Rule(config.BindWhen(RiskyModSource.FreezeExecutesBosses, section, "Cook Execute", true, executeDescription));

        _hitTracking.Add(execute, execute);
        execute.Subscribe(() => ExecuteAPI.CalculateExecuteThreshold += ExecuteCookHits, () => ExecuteAPI.CalculateExecuteThreshold -= ExecuteCookHits);

        var healthBarDescription = new Description("CHEF's health bars show Cook's execute threshold again.");
        var healthBar = new Rule(config.Bind(section, "Cook Health Bar", true, healthBarDescription));

        _hitTracking.Add(healthBar, healthBar);
        healthBar.Subscribe(() => ExecuteAPI.CalculateExecuteThresholdForViewer += ShowCookThreshold, () => ExecuteAPI.CalculateExecuteThresholdForViewer -= ShowCookThreshold);
    }

    private static void TrackResolvingHit(Action<DamageReport> orig, DamageReport damageReport)
    {
        var outerHit = _resolvingHit;
        _resolvingHit = damageReport;

        try
        {
            orig(damageReport);
        }
        finally
        {
            _resolvingHit = outerHit;
        }
    }

    private static void ExecuteCookHits(CharacterBody victimBody, ref float highestExecuteThreshold)
    {
        if (_resolvingHit != null && DamageAPI.HasModdedDamageType(_resolvingHit.damageInfo, CookDamageType.DamageType))
            highestExecuteThreshold = Mathf.Max(highestExecuteThreshold, ChefConfig.Execute_Threshold.Value);
    }

    private static void ShowCookThreshold(CharacterBody victimBody, CharacterBody viewerBody, ref float highestExecuteThreshold)
    {
        if (_resolvingHit == null && WieldsCook(viewerBody))
            highestExecuteThreshold = Mathf.Max(highestExecuteThreshold, ChefConfig.Execute_Threshold.Value);
    }

    private static bool WieldsCook(CharacterBody body) => body.skillLocator && body.skillLocator.special && body.skillLocator.special.skillDef == SpecialCookSkill.SkillDef;
}
