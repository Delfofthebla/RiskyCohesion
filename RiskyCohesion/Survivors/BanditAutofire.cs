using System;
using System.Linq;
using BepInEx.Configuration;
using EggsSkills.Config;
using EggsSkills.EntityStates;
using EntityStates;
using EntityStates.Bandit;
using EntityStates.Bandit2.Weapon;
using RiskyCohesion.Sources;
using RiskyTweaks.Tweaks.Survivors.Bandit2;
using OrigGetMinimumInterruptPriority = On.EntityStates.Bandit2.Weapon.Bandit2FirePrimaryBase.orig_GetMinimumInterruptPriority;
using OrigOnEnter = On.EntityStates.Bandit2.Weapon.Bandit2FirePrimaryBase.orig_OnEnter;

namespace RiskyCohesion.Survivors;

internal static class BanditAutofire
{
    private static readonly SharedHook<Type> _minimumDurations = new(typeof(PrimaryAutoFire), nameof(PrimaryAutoFire.Bandit2FirePrimaryBase_OnEnter), KeepMinimumDuration);
    private static readonly SharedHook<Type> _interruptPriorities = new(typeof(PrimaryAutoFire), nameof(PrimaryAutoFire.Bandit2FirePrimaryBase_GetMinimumInterruptPriority), KeepInterruptPriority);

    public static void InstallTranquilizerGun(ConfigFile config) => Exclude(config, ContentMod.Starstorm, "Tranquilizer Gun", typeof(TranquilizerGun), true);

    public static void InstallMagicBullet(ConfigFile config)
        => Exclude(config, ContentMod.EggsSkills, "Magic Bullet", typeof(MagicBulletEntity), Configuration.GetConfigValue(Configuration.EnableBanditSkills));

    private static void Exclude(ConfigFile config, ContentMod contentMod, string skillName, Type fireState, bool skillIsEnabled)
    {
        var description = new Description($"{skillName} keeps {contentMod.Name}'s fire rate instead of RiskyTweaks' Primary Autofire fire modes.")
            .Requires(RiskyTweaksSource.BanditPrimaryAutofireSetting);

        var keepFireRate = new Rule(config.BindWhen(skillIsEnabled, contentMod.SurvivorsSection, $"{skillName} Fire Rate", true, description));

        _minimumDurations.Add(fireState, keepFireRate);
        _interruptPriorities.Add(fireState, keepFireRate);
    }

    private static void KeepMinimumDuration(Action<PrimaryAutoFire, OrigOnEnter, Bandit2FirePrimaryBase> orig, PrimaryAutoFire autofire, OrigOnEnter next, Bandit2FirePrimaryBase fire)
    {
        if (_minimumDurations.Active.Contains(fire.GetType()))
            next(fire);
        else
            orig(autofire, next, fire);
    }

    private static InterruptPriority KeepInterruptPriority(
        Func<PrimaryAutoFire, OrigGetMinimumInterruptPriority, Bandit2FirePrimaryBase, InterruptPriority> orig,
        PrimaryAutoFire autofire,
        OrigGetMinimumInterruptPriority next,
        Bandit2FirePrimaryBase fire)
    {
        if (_interruptPriorities.Active.Contains(fire.GetType()))
            return next(fire);

        return orig(autofire, next, fire);
    }
}
