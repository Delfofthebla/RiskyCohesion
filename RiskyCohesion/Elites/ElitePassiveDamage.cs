using System;
using BepInEx.Configuration;
using RiskyCohesion.Sources;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace RiskyCohesion.Elites;

internal sealed class ElitePassiveDamage(ConfigEntry<bool> enabled) : Rule(enabled)
{
    public static ElitePassiveDamage Bind(ConfigFile config, ContentMod contentMod, string eliteName, EliteRework rework, Description description)
        => BindEntry(config, contentMod, $"{eliteName} Passive Damage", rework, description);

    public static ElitePassiveDamage BindZones(ConfigFile config, ContentMod contentMod, string eliteName, Description description)
        => BindEntry(config, contentMod, $"{eliteName} Zones", EliteRework.Blazing, description);

    public void ReplaceProjectileDamage(Func<GameObject> projectilePrefab, FlatDamage flatDamage)
        => new SpawnedProjectile(projectilePrefab, (controller, projectile) => ReplaceDamage(controller, projectile.owner, flatDamage)).Hook(this);

    public void SpreadOverOneSecond(ProjectileDotZone zone)
    {
        var hitsPerSecond = Math.Min(zone.fireFrequency, zone.resetFrequency);
        if (hitsPerSecond > 1f)
            Override(() => zone.damageCoefficient, value => zone.damageCoefficient = value, original => original / hitsPerSecond);
    }

    public void RemoveProcs(ProjectileDotZone zone) => Override(() => zone.overlapProcCoefficient, value => zone.overlapProcCoefficient = value, _ => 0f);

    public void RemoveProcs(ProjectileController projectile) => Override(() => projectile.procCoefficient, value => projectile.procCoefficient = value, _ => 0f);

    private static ElitePassiveDamage BindEntry(ConfigFile config, ContentMod contentMod, string key, EliteRework rework, Description description)
        => new(config.BindWhen(rework.IsOn, contentMod.ElitesSection, key, true, description.Requires(rework.Requirement)));

    private static void ReplaceDamage(ProjectileController controller, GameObject owner, FlatDamage flatDamage)
    {
        var projectileDamage = controller.GetComponent<ProjectileDamage>();
        if (projectileDamage)
            projectileDamage.damage = flatDamage.Replace(projectileDamage.damage, owner.ComponentOrNull<CharacterBody>());
    }
}
