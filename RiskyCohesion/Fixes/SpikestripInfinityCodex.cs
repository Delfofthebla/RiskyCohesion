using System;
using BepInEx.Configuration;
using GrooveSaladSpikestripContent.Content;
using RoR2;
using Spikestrip2_0.Core;
using UnityEngine;
using OrigTakeDamage = On.RoR2.HealthComponent.orig_TakeDamage;

namespace RiskyCohesion.Fixes;

internal static class SpikestripInfinityCodex
{
    public static void Install(ConfigFile config)
    {
        if (!SpikestripContent.IsEnabled<InfinityCodex>())
            return;

        var description = new Description("Infinity Codex's bonus damage works for every player, not only the host.");
        var everyPlayer = new Rule(config.Bind(ContentMod.Spikestrip.ItemsSection, "Infinity Codex", true, description));

        everyPlayer.Hook(typeof(InfinityCodex), nameof(InfinityCodex.HealthComponent_TakeDamage), ApplyForRemotePlayers);
    }

    private static void ApplyForRemotePlayers(
        Action<InfinityCodex, OrigTakeDamage, HealthComponent, DamageInfo> orig,
        InfinityCodex self,
        OrigTakeDamage next,
        HealthComponent victim,
        DamageInfo damageInfo)
    {
        var stack = RemotePlayerStack(damageInfo.attacker, victim);
        if (stack == 0)
        {
            orig(self, next, victim, damageInfo);
            return;
        }

        var multiplier = 1f + ItemValues.InfinityCodexDamagePerStack * stack;

        damageInfo.damage *= multiplier;
        damageInfo.damageColorIndex = InfinityCodex.codexColorIndex;
        EffectManager.SimpleImpactEffect(InfinityCodex.impactEffect, damageInfo.position, -damageInfo.force, true);

        next(victim, damageInfo);

        damageInfo.damage /= multiplier;
    }

    private static int RemotePlayerStack(GameObject attacker, HealthComponent victim)
    {
        var body = attacker.ComponentOrNull<CharacterBody>();
        var master = body ? body.master : null;
        var player = master ? master.playerCharacterMasterController : null;
        var networkUser = player ? player.networkUser : null;

        if (!networkUser || networkUser.localUser != null || !master.inventory)
            return 0;

        var stack = master.inventory.GetItemCountEffective(InfinityCodex.instance.ItemDef);

        return stack > 0 && HasLogged(networkUser, victim)
            ? stack
            : 0;
    }

    private static bool HasLogged(NetworkUser networkUser, HealthComponent victim)
        => victim.TryGetComponent<DeathRewards>(out var rewards) && rewards.logUnlockableDef && networkUser.unlockables.Contains(rewards.logUnlockableDef);
}
