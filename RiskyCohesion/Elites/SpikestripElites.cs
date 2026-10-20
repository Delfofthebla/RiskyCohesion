using System;
using BepInEx.Configuration;
using GrooveSaladSpikestripContent.Content;
using PlasmaCoreSpikestripContent.Components;
using PlasmaCoreSpikestripContent.Content.Elites;
using RiskyCohesion.Sources;
using RoR2;
using Spikestrip2_0.Core;
using AragoniteBehaviour = PlasmaCoreSpikestripContent.Content.Elites.RagingElite.RageAffixBuffBehaviourServer;
using PlatedBehaviour = GrooveSaladSpikestripContent.Content.PlatedElite.PlatedAffixBuffBehaviour;
using WarpedBehaviour = GrooveSaladSpikestripContent.Content.WarpedElite.GravityAffixBuffBehaviourServer;

namespace RiskyCohesion.Elites;

internal static class SpikestripElites
{
    private static readonly float DeathStrikeDamageCoefficient = 5f;

    public static void Install(ConfigFile config)
    {
        if (SpikestripContent.IsEnabled<WarpedElite>())
            InstallWarped(config);

        if (SpikestripContent.IsEnabled<PlatedElite>())
            InstallPlated(config);

        if (SpikestripContent.IsEnabled<CloakedElite>())
            BindStats(config, "Veiled", CloakedElite.instance.MainEliteDef, EliteTier.Tier1);

        if (SpikestripContent.IsEnabled<RagingElite>())
            InstallAragonite(config);
    }

    private static void InstallWarped(ConfigFile config)
    {
        BindStats(config, "Warped", WarpedElite.instance.MainEliteDef, EliteTier.Tier1);

        var stun = ElitePassiveSuppression.Bind(config, ContentMod.Spikestrip, "Warped", "Warped's projectile deflection");
        new StunResetTimer<WarpedBehaviour>(stun, behaviour => behaviour.body, behaviour => behaviour.rechargeTimer = behaviour.currentRechargeTime).Install();
    }

    private static void InstallPlated(ConfigFile config)
    {
        BindStats(config, "Plated", PlatedElite.instance.MainEliteDef, EliteTier.Tier1);

        var stun = ElitePassiveSuppression.Bind(config, ContentMod.Spikestrip, "Plated", "Plated's plating");
        new StunSkippedDamageReaction<PlatedBehaviour>(stun, behaviour => behaviour.body).Install();
    }

    private static void InstallAragonite(ConfigFile config)
    {
        BindStats(config, "Aragonite", RagingElite.instance.MainEliteDef, EliteTier.Tier2);

        var stun = ElitePassiveSuppression.Bind(config, ContentMod.Spikestrip, "Aragonite", "Aragonite's ally buff ward and its strikes on attackers");
        new StunResetTimer<AragoniteBehaviour>(stun, behaviour => behaviour.body, behaviour => behaviour.currentRadius = 0f).Install();
        new StunSkippedDamageReaction<AragoniteBehaviour>(stun, behaviour => behaviour.body).Install();

        var description = new Description("Aragonite's strikes deal Malachite's spike damage.");
        var strikes = ElitePassiveDamage.Bind(config, ContentMod.Spikestrip, "Aragonite", EliteRework.Malachite, description);

        strikes.ReplaceProjectileDamage(() => RagingElite.deathProjectile, EliteReworksSource.MalachiteSpikes);
        strikes.Hook(typeof(RagingEliteProjectileSpawner), nameof(RagingEliteProjectileSpawner.FixedUpdate), ReplaceDeathStrikeDamage);
    }

    private static void BindStats(ConfigFile config, string eliteName, EliteDef eliteDef, EliteTier tier)
        => EliteStats.Bind(config, ContentMod.Spikestrip, eliteName, eliteDef.name, tier, eliteDef.healthBoostCoefficient, eliteDef.damageBoostCoefficient);

    // The dead elite's body can be destroyed before the spawner's last wave, so the damage is replaced once, before the first.
    private static void ReplaceDeathStrikeDamage(Action<RagingEliteProjectileSpawner> orig, RagingEliteProjectileSpawner self)
    {
        if (self.spawnedWaves == 0)
            self.ownerBodyDamage = DeathStrikeDamage(self) / DeathStrikeDamageCoefficient;

        orig(self);
    }

    private static float DeathStrikeDamage(RagingEliteProjectileSpawner spawner)
        => EliteReworksSource.MalachiteSpikes.Replace(spawner.ownerBodyDamage * DeathStrikeDamageCoefficient, spawner.owner.ComponentOrNull<CharacterBody>());
}
