using System;
using BepInEx.Configuration;
using PlasmaCoreSpikestripContent.Components;
using PlasmaCoreSpikestripContent.Content.Elites;
using RoR2;
using RoR2.Projectile;
using Spikestrip2_0.Core;
using UnityEngine;

namespace RiskyCohesion.Fixes;

internal static class SpikestripAragoniteDeathStrikes
{
    public static void Install(ConfigFile config)
    {
        if (!SpikestripContent.IsEnabled<RagingElite>())
            return;

        var description = new Description("Aragonite's death strikes only hit its enemies.");
        var fireOnOwnersTeam = new Rule(config.Bind(ContentMod.Spikestrip.ElitesSection, "Aragonite Death Strikes", true, description));

        fireOnOwnersTeam.Hook(typeof(RagingEliteProjectileSpawner), nameof(RagingEliteProjectileSpawner.FixedUpdate), RememberOwnersTeam);
        new SpawnedProjectile(() => RagingElite.deathProjectile, FireOnOwnersTeam).Hook(fireOnOwnersTeam);
    }

    // The dead elite's body can be destroyed before the spawner's last wave, so its team is kept on the spawner from the first.
    private static void RememberOwnersTeam(Action<RagingEliteProjectileSpawner> orig, RagingEliteProjectileSpawner self)
    {
        if (self.spawnedWaves == 0)
            self.gameObject.AddComponent<OwnersTeam>().Index = TeamComponent.GetObjectTeam(self.owner);

        orig(self);
    }

    private static void FireOnOwnersTeam(ProjectileController controller, FireProjectileInfo projectile)
    {
        var ownersTeam = projectile.owner.ComponentOrNull<OwnersTeam>();
        if (ownersTeam)
            controller.teamFilter.teamIndex = ownersTeam.Index;
    }

    private sealed class OwnersTeam : MonoBehaviour
    {
        public TeamIndex Index { get; set; }
    }
}
