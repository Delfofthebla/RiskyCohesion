using System;
using BepInEx.Configuration;
using RiskyCohesion.Sources;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace RiskyCohesion.BandProcs;

internal sealed class ItemBandProc(ConfigEntry<bool> enabled) : Rule(enabled)
{
    public static ItemBandProc Bind(ConfigFile config, ContentMod contentMod, string itemName, string attack)
    {
        var description = new Description($"{attack} can't trigger elemental bands.")
            .Requires(RiskyTweaksSource.ShurikenNoBandProcSetting);

        return new ItemBandProc(config.BindWhen(RiskyTweaksSource.ShurikenSkipsBands, contentMod.ItemsSection, itemName, true, description));
    }

    public void RemoveFrom(Func<GameObject> projectilePrefab) => new SpawnedProjectile(projectilePrefab, RemoveBandProc).Hook(this);

    private static void RemoveBandProc(ProjectileController controller, FireProjectileInfo projectile)
    {
        if (RiskyTweaksSource.ShurikenSkipsBandsOnlyForPlayers && !IsPlayer(projectile.owner))
            return;

        var procChainMask = controller.procChainMask;
        procChainMask.AddProc(ProcType.Rings);
        controller.procChainMask = procChainMask;
    }

    private static bool IsPlayer(GameObject owner)
    {
        var body = owner.ComponentOrNull<CharacterBody>();

        return body && body.isPlayerControlled;
    }
}
