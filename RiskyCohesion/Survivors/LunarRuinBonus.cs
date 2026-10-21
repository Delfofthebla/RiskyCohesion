using System;
using System.Linq;
using BepInEx.Configuration;
using RiskyCohesion.Sources;
using RiskyFixes.Fixes.Survivors.FalseSon;
using RoR2;
using StormboundFalseSon;
using UnityEngine;
using DamageModifierArgs = SneedHooks.ModifyFinalDamage.DamageModifierArgs;
using FinalDamageModifiers = SneedHooks.ModifyFinalDamage;

namespace RiskyCohesion.Survivors;

internal static class LunarRuinBonus
{
    private enum Change
    {
        StormboundCap,
        AddsToLinearDamage,
        SkipsDelayedHits,
    }

    private static readonly Delegate RiskyFixesBonus = FinalDamageModifiers.ModifyFinalDamageActions?.GetInvocationList().FirstOrDefault(action => action.Target is LunarRuinReproc);
    private static readonly bool RiskyFixesAddsBonus = RiskyFixesBonus?.Method.Name == nameof(LunarRuinReproc.ModifyFinalDamage_Additive);

    private static readonly SharedHook<Change> _bonus = new(typeof(LunarRuinReproc), RiskyFixesBonus?.Method.Name, ApplyBonus);

    public static void InstallStormboundCap(ConfigFile config)
    {
        var description = new Description("RiskyFixes' Lunar Ruin damage bonus follows StormboundFalseSon's Lunar Ruin Damage Cap.")
            .Requires(RiskyFixesSource.LunarRuinReprocSetting);

        var cap = new Rule(config.BindWhen(RiskyFixesBonus != null, ContentMod.StormboundFalseSon.SurvivorsSection, "Lunar Ruin Cap", true, description));

        _bonus.Add(Change.StormboundCap, cap);
    }

    public static void InstallFixes(ConfigFile config)
    {
        var additiveDescription = new Description("RiskyFixes' Lunar Ruin damage bonus adds to LinearDamage's item bonuses instead of multiplying them.")
            .Requires(RiskyFixesSource.LunarRuinReprocSetting);

        var multipliesLinearDamage = RiskyFixesBonus != null && !RiskyFixesAddsBonus && Mods.IsLoaded(Mods.LINEAR_DAMAGE);
        var additive = new Rule(config.BindWhen(multipliesLinearDamage, Sections.Vanilla, "Lunar Ruin Additive", true, additiveDescription));

        var delayedHitsDescription = new Description("Warped Echo's delayed damage no longer gets RiskyFixes' Lunar Ruin bonus a second time.")
            .Requires(RiskyFixesSource.LunarRuinReprocSetting);

        var skipDelayedHits = new Rule(config.BindWhen(RiskyFixesBonus != null, Sections.Vanilla, "Warped Echo Lunar Ruin", true, delayedHitsDescription));

        _bonus.Add(Change.AddsToLinearDamage, additive);
        _bonus.Add(Change.SkipsDelayedHits, skipDelayedHits);
    }

    private static void ApplyBonus(
        Action<LunarRuinReproc, DamageModifierArgs, DamageInfo, HealthComponent, CharacterBody> orig,
        LunarRuinReproc riskyFixes,
        DamageModifierArgs damageModifierArgs,
        DamageInfo damageInfo,
        HealthComponent victim,
        CharacterBody victimBody)
    {
        var changes = _bonus.Active;
        if (damageInfo.delayedDamageSecondHalf && changes.Contains(Change.SkipsDelayedHits))
            return;

        if (changes.Contains(Change.StormboundCap))
            ApplyCappedBonus(damageModifierArgs, damageInfo, victimBody, changes.Contains(Change.AddsToLinearDamage));
        else if (changes.Contains(Change.AddsToLinearDamage))
            riskyFixes.ModifyFinalDamage_Additive(damageModifierArgs, damageInfo, victim, victimBody);
        else
            orig(riskyFixes, damageModifierArgs, damageInfo, victim, victimBody);
    }

    private static void ApplyCappedBonus(DamageModifierArgs damageModifierArgs, DamageInfo damageInfo, CharacterBody victimBody, bool addsToLinearDamage)
    {
        var stacks = victimBody.GetBuffCount(DLC2Content.Buffs.lunarruin);
        if (stacks <= 0)
            return;

        var bonus = StormboundCapped(LunarRuinReproc.damageMultPerBuff * stacks);
        if (RiskyFixesAddsBonus || addsToLinearDamage)
            damageModifierArgs.damageMultAdd += bonus;
        else
            damageModifierArgs.damageMultFinal *= 1f + bonus;

        damageInfo.damageColorIndex = DamageColorIndex.Void;
    }

    private static float StormboundCapped(float linearBonus)
    {
        if (!Configs.lunarRuinEnable)
            return linearBonus;

        var band = Configs.lunarRuinCap / 2f;
        var bonus = 0f;
        var weight = 1f;

        for (var remaining = linearBonus; remaining > 0f && band > 0f; remaining -= band)
        {
            bonus += Mathf.Min(band, remaining) * weight;
            weight /= 2f;
        }

        return bonus;
    }
}
