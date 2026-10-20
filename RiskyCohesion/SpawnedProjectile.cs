using System;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyCohesion;

internal sealed class SpawnedProjectile(Func<GameObject> prefab, Action<ProjectileController, FireProjectileInfo> change)
{
    private static readonly SharedHook<SpawnedProjectile> _initializeProjectile = new(typeof(ProjectileManager), nameof(ProjectileManager.InitializeProjectile), InitializeProjectile);

    public void Hook(Rule rule) => _initializeProjectile.Add(this, rule);

    private static void InitializeProjectile(Action<ProjectileController, FireProjectileInfo> orig, ProjectileController controller, FireProjectileInfo projectile)
    {
        orig(controller, projectile);

        if (!NetworkServer.active)
            return;

        foreach (var spawned in _initializeProjectile.Active)
            spawned.ChangeIfFromPrefab(controller, projectile);
    }

    private void ChangeIfFromPrefab(ProjectileController controller, FireProjectileInfo projectile)
    {
        if (projectile.projectilePrefab == prefab())
            change(controller, projectile);
    }
}
