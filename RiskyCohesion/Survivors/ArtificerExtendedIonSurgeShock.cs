using System;
using ArtificerExtended;
using ArtificerExtended.States;
using BepInEx.Configuration;
using R2API;
using RiskyCohesion.Sources;
using RoR2;
using RoR2.Orbs;
using UnityEngine;

namespace RiskyCohesion.Survivors;

internal static class ArtificerExtendedIonSurgeShock
{
    private static readonly string RiskyModDescriptionToken = "MAGE_SPECIAL_LIGHTNING_DESCRIPTION_RISKYMOD";

    private static bool _zapPending;
    private static bool _impactPending;
    private static LanguageAPI.LanguageOverlay _shockingText;

    public static void Install(ConfigFile config)
    {
        var description = new Description("ArtificerExtended's Ion Surge shocks enemies instead of stunning them.")
            .Requires(RiskyModSource.IonSurgeShockSetting);

        var sourceIsOn = RiskyModSource.ShocksWithIonSurge && !ArtificerExtendedPlugin.LegacyIonSurge;
        var shock = new Rule(config.BindWhen(sourceIsOn, ContentMod.ArtificerExtended.SurvivorsSection, "Ion Surge Shock", true, description));

        shock.Hook(typeof(SurgeExtendedDash), nameof(SurgeExtendedDash.OnEnter), ShockOnGraze);
        shock.Hook(typeof(SurgeExtendedDash), nameof(SurgeExtendedDash.OnZapTargetFound), ShockOnZap);
        shock.Hook(typeof(OrbManager), nameof(OrbManager.AddOrb), ShockPendingZap);
        shock.Hook(typeof(SurgeExtendedImpact), nameof(SurgeExtendedImpact.OnEnter), ShockOnImpact);
        shock.Hook(typeof(BlastAttack), nameof(BlastAttack.Fire), ShockPendingImpact);
        shock.Subscribe(ShowShockingText, HideShockingText);
    }

    private static void ShockOnGraze(Action<SurgeExtendedDash> orig, SurgeExtendedDash dash)
    {
        orig(dash);

        dash.attack.damageType = Shocking(dash.attack.damageType);
    }

    private static void ShockOnZap(Action<SurgeExtendedDash, Vector3, HurtBox> orig, SurgeExtendedDash dash, Vector3 position, HurtBox target)
    {
        _zapPending = true;

        try
        {
            orig(dash, position, target);
        }
        finally
        {
            _zapPending = false;
        }
    }

    private static void ShockPendingZap(Action<OrbManager, Orb> orig, OrbManager orbs, Orb orb)
    {
        if (_zapPending && orb is LightningOrb zap)
        {
            _zapPending = false;
            zap.damageType = Shocking(zap.damageType);
        }

        orig(orbs, orb);
    }

    private static void ShockOnImpact(Action<SurgeExtendedImpact> orig, SurgeExtendedImpact impact)
    {
        _impactPending = true;

        try
        {
            orig(impact);
        }
        finally
        {
            _impactPending = false;
        }
    }

    private static BlastAttack.Result ShockPendingImpact(Func<BlastAttack, BlastAttack.Result> orig, BlastAttack blast)
    {
        if (_impactPending)
        {
            _impactPending = false;
            blast.damageType = Shocking(blast.damageType);
        }

        return orig(blast);
    }

    private static DamageTypeCombo Shocking(DamageTypeCombo damageType)
    {
        damageType.damageType = (damageType.damageType & ~DamageType.Stun1s) | DamageType.Shock5s;
        return damageType;
    }

    private static void ShowShockingText()
    {
        var shockingDescription = ArtificerExtendedPlugin.surgeFullDesc.Replace("Stunning", "Shocking");

        _shockingText = LanguageAPI.AddOverlay(RiskyModDescriptionToken, shockingDescription);
    }

    private static void HideShockingText()
    {
        _shockingText?.Remove();
        _shockingText = null;
    }
}
