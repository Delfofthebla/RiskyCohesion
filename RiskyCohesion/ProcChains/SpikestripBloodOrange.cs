using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using GrooveSaladSpikestripContent.Content;
using R2API;
using RiskyCohesion.Sources;
using RoR2;
using UnityEngine;
using OrigTakeDamage = On.RoR2.HealthComponent.orig_TakeDamage;

namespace RiskyCohesion.ProcChains;

internal static class SpikestripBloodOrange
{
    private static readonly string Name = "Blood Orange";
    private static readonly string ReworkKey = $"{Name} Rework";
    private static readonly float DefaultBonus = 0.3f;
    private static readonly float DefaultBonusPerStack = 0.15f;
    private static readonly AcceptableValueRange<float> BonusRange = new(0f, 1f);

    private static readonly string Pickup = "Your damage-over-time effects grow stronger with each other type on the enemy. High damage hits also inflict damage-over-time.";

    private static readonly HashSet<DotController.DotIndex> _otherDotTypes = [];

    private static Rule _rework;
    private static ConfigEntry<float> _bonus;
    private static ConfigEntry<float> _bonusPerStack;
    private static LanguageAPI.LanguageOverlay _reworkText;

    public static bool IsReworked => _rework is { SourceIsOn: true, IsOn: true };

    public static void Install(ConfigFile config)
    {
        var section = ContentMod.Spikestrip.ItemsSection;

        var reworkDescription = new Description($"{Name} makes your damage-over-time effects stronger for each other type on the enemy, instead of making them trigger On-Hit items.")
            .Requires(RiskyModSource.DisableProcChainsSetting);

        _rework = new Rule(config.BindWhen(RiskyModSource.DisablesProcChains, section, ReworkKey, true, reworkDescription));
        if (!_rework.SourceIsOn)
            return;

        var bonusDescription = new Description($"{Name}'s damage bonus for each other type of damage-over-time, at one stack.")
            .Requires(ReworkKey);

        var bonusPerStackDescription = new Description($"{Name}'s damage bonus for each other type of damage-over-time, per additional stack.")
            .Requires(ReworkKey);

        _bonus = config.Bind(section, $"{ReworkKey} Bonus", DefaultBonus, new ConfigDescription(bonusDescription, BonusRange));
        _bonusPerStack = config.Bind(section, $"{ReworkKey} Bonus Per Stack", DefaultBonusPerStack, new ConfigDescription(bonusPerStackDescription, BonusRange));

        _rework.Hook(typeof(DotProcs), nameof(DotProcs.HealthComponent_TakeDamage), BoostDotDamage);
        _rework.Subscribe(ShowReworkText, HideReworkText);
        _bonus.SettingChanged += RefreshReworkText;
        _bonusPerStack.SettingChanged += RefreshReworkText;
    }

    public static float Bonus(int stack) => _bonus.Value + _bonusPerStack.Value * (stack - 1);

    public static void OnReworkToggled(Action change)
    {
        if (_rework is { SourceIsOn: true })
            _rework.Enabled.SettingChanged += (_, _) => change();
    }

    private static void BoostDotDamage(Action<DotProcs, OrigTakeDamage, HealthComponent, DamageInfo> orig, DotProcs self, OrigTakeDamage next, HealthComponent victim, DamageInfo damageInfo)
    {
        if (damageInfo.dotIndex != DotController.DotIndex.None)
            damageInfo.damage *= DotDamageMultiplier(damageInfo);

        next(victim, damageInfo);
    }

    private static float DotDamageMultiplier(DamageInfo tick)
    {
        var stack = BloodOrangeStack(tick.attacker);
        if (stack == 0)
            return 1f;

        return 1f + Bonus(stack) * OtherDotTypes(tick);
    }

    private static int OtherDotTypes(DamageInfo tick)
    {
        var dots = tick.inflictor.ComponentOrNull<DotController>();
        if (!dots)
            return 0;

        _otherDotTypes.Clear();
        foreach (var dot in dots.dotStackList)
        {
            if (dot.attackerObject == tick.attacker && dot.dotIndex != tick.dotIndex)
                _otherDotTypes.Add(dot.dotIndex);
        }

        return _otherDotTypes.Count;
    }

    private static int BloodOrangeStack(GameObject holder)
    {
        var body = holder.ComponentOrNull<CharacterBody>();

        return body && body.inventory
            ? body.inventory.GetItemCountEffective(DotProcs.instance.ItemDef)
            : 0;
    }

    private static void ShowReworkText()
    {
        var bloodOrange = DotProcs.instance.ItemDef;

        _reworkText = LanguageAPI.AddOverlay(new Dictionary<string, string>
        {
            [bloodOrange.descriptionToken] = ItemDescription(),
            [bloodOrange.pickupToken] = Pickup,
        });
    }

    private static void HideReworkText()
    {
        _reworkText?.Remove();
        _reworkText = null;
    }

    private static void RefreshReworkText(object sender, EventArgs args)
    {
        if (!_rework.IsOn)
            return;

        HideReworkText();
        ShowReworkText();
    }

    private static string ItemDescription()
    {
        var bonus = $"<style=cIsDamage>+{Percent(_bonus.Value)}%</style> <style=cStack>(+{Percent(_bonusPerStack.Value)}% per stack)</style>";

        return $"Your damage-over-time effects deal {bonus} damage for each <style=cIsDamage>other type of damage-over-time</style> you've inflicted on the enemy."
            + " Hits that deal <style=cIsDamage>more than 400% damage</style> also deal an additional <style=cIsDamage>600% damage</style> over time.";
    }

    private static string Percent(float fraction) => Description.Format(fraction * 100f);
}
