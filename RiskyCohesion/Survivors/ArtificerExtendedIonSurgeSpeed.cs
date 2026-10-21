using System;
using ArtificerExtended;
using ArtificerExtended.States;
using BepInEx.Configuration;
using RiskyCohesion.Sources;
using RoR2;
using UnityEngine;

namespace RiskyCohesion.Survivors;

internal static class ArtificerExtendedIonSurgeSpeed
{
    public static void Install(ConfigFile config)
    {
        var description = new Description("ArtificerExtended's Ion Surge flies at the same speed regardless of movement speed.")
            .Requires(RiskyTweaksSource.IonSurgeMoveSpeedSetting);

        var sourceIsOn = RiskyTweaksSource.IonSurgeIgnoresMoveSpeed && !ArtificerExtendedPlugin.LegacyIonSurge;
        var fixedSpeed = new Rule(config.BindWhen(sourceIsOn, ContentMod.ArtificerExtended.SurvivorsSection, "Ion Surge Fixed Speed", true, description));

        fixedSpeed.Hook(typeof(SurgeExtendedDash), nameof(SurgeExtendedDash.GetIdealVelocity), VelocityAtBaseSpeed);
    }

    private static Vector3 VelocityAtBaseSpeed(Func<SurgeExtendedDash, bool, Vector3> orig, SurgeExtendedDash dash, bool useGravity)
    {
        var body = dash.characterBody;
        if (body.moveSpeed <= 0f)
            return orig(dash, useGravity);

        var flight = orig(dash, false);
        var fall = orig(dash, useGravity) - flight;

        return flight * (BaseSpeed(body) / body.moveSpeed) + fall;
    }

    private static float BaseSpeed(CharacterBody body) => body.isSprinting
        ? body.baseMoveSpeed * body.sprintingSpeedMultiplier
        : body.baseMoveSpeed;
}
