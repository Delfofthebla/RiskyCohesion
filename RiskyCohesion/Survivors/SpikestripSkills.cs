using System;
using BepInEx.Configuration;
using GrooveSaladSpikestripContent.Content;
using PlasmaCoreSpikestripContent.Content.Skills;
using RiskyCohesion.Sources;
using RoR2;
using Spikestrip2_0.Core;
using UnityEngine;
using SuppressBlightComponent = RiskyMod.Survivors.Croco.Tweaks.NerfCloyingBreath.SuppressBlightComponent;

namespace RiskyCohesion.Survivors;

internal static class SpikestripSkills
{
    private static readonly DamageType DeeprotDamage = DamageType.PoisonOnHit | DamageType.BlightOnHit;

    public static void InstallDeeprot(ConfigFile config)
    {
        var description = new Description("RiskyMod's Cloying Breath nerf also applies to Deeprot.")
            .Requires(RiskyModSource.CloyingBreathNerfSetting);

        var nerfDeeprot = new Rule(config.BindWhen(SpikestripContent.IsEnabled<DeepRot>(), ContentMod.Spikestrip.SurvivorsSection, "Deeprot", true, description));

        nerfDeeprot.Hook(typeof(SuppressBlightComponent), nameof(SuppressBlightComponent.Start), SuppressDeeprot);
        nerfDeeprot.Hook(typeof(SuppressBlightComponent), nameof(SuppressBlightComponent.FixedUpdate), RestoreDeeprot);
    }

    public static void InstallConduit(ConfigFile config)
    {
        var description = new Description("Conduit keeps sprinting, like RiskyTweaks' Loader skills.")
            .Requires(RiskyTweaksSource.LoaderSprintSetting);

        var sourceIsOn = RiskyTweaksSource.LoaderSkillsKeepSprint && SpikestripContent.IsEnabled<LoaderVoidBeam>();
        var keepSprinting = new Rule(config.BindWhen(sourceIsOn, ContentMod.Spikestrip.SurvivorsSection, "Conduit", true, description));
        if (!keepSprinting.SourceIsOn)
            return;

        var conduit = ContentBase<LoaderVoidBeam>.instance.SkillDef;

        keepSprinting.Override(() => conduit.cancelSprintingOnActivation, cancels => conduit.cancelSprintingOnActivation = cancels, _ => false);
    }

    private static void SuppressDeeprot(Action<SuppressBlightComponent> orig, SuppressBlightComponent suppressor)
    {
        var damage = suppressor.projectileDamage;
        if (damage.damageType.damageType == DeeprotDamage)
        {
            damage.damageType.damageType = DamageType.BlightOnHit;
            suppressor.gameObject.AddComponent<DeeprotBreath>();
        }

        orig(suppressor);
    }

    private static void RestoreDeeprot(Action<SuppressBlightComponent> orig, SuppressBlightComponent suppressor)
    {
        orig(suppressor);

        var damage = suppressor.projectileDamage;
        if (damage.damageType.damageType == DamageType.BlightOnHit && suppressor.TryGetComponent<DeeprotBreath>(out _))
            damage.damageType.damageType = DeeprotDamage;
    }

    private sealed class DeeprotBreath : MonoBehaviour
    {
    }
}
