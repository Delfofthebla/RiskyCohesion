using BepInEx.Configuration;
using RiskyCohesion.Sources;
using RoR2;
using RoR2.Projectile;
using SS2;
using SS2.Components;
using SS2.Equipments;
using UnityEngine;

namespace RiskyCohesion.Elites;

internal static class StarstormPassiveDamage
{
    private static ElitePassiveDamage _etherealZones;
    private static ElitePassiveDamage _tectonicZones;
    private static ElitePassiveDamage _infernalZones;
    private static ElitePassiveDamage _borealZones;

    public static void Install(ConfigFile config)
    {
        var etherealDamage = ElitePassiveDamage.Bind(config, ContentMod.Starstorm, "Ethereal", EliteRework.Malachite, new Description("Ethereal's circles deal Malachite's spike damage."));
        etherealDamage.ReplaceProjectileDamage(() => AffixEthereal.EtherealBehavior.projectilePrefab, EliteReworksSource.MalachiteSpikes);

        _etherealZones = ElitePassiveDamage.BindZones(config, ContentMod.Starstorm, "Ethereal", ZoneDescription("Ethereal's circles"));
        _tectonicZones = ElitePassiveDamage.BindZones(config, ContentMod.Starstorm, "Tectonic", ZoneDescription("Tectonic's pillars"));
        _infernalZones = ElitePassiveDamage.BindZones(config, ContentMod.Starstorm, "Infernal", ZoneDescription("Infernal's fire pools"));
        _borealZones = ElitePassiveDamage.BindZones(config, ContentMod.Starstorm, "Boreal", new Description("Boreal's slowing zones have proc coefficient 0."));

        if (EliteRework.Blazing.IsOn)
            RoR2Application.onLoad += () => Failsafe.Run("reworking Starstorm 2's elite damage zones", ReworkZones);
    }

    private static Description ZoneDescription(string zones) => new($"{zones} deal one hit's worth of damage per second, with proc coefficient 0.");

    private static void ReworkZones()
    {
        ReworkZone(_etherealZones, ChildProjectile(AffixEthereal.EtherealBehavior.projectilePrefab));
        ReworkZone(_tectonicZones, ChildProjectile(AffixSuperEarth.projectilePrefab));
        ReworkInfernalZones(AffixSuperFire.projectilePrefab.ComponentOrNull<SuperFireballController>());
        ReworkBorealZone(ChildProjectile(AffixSuperIce.projectilePrefab));
    }

    private static void ReworkInfernalZones(SuperFireballController fireball)
    {
        if (!fireball)
            return;

        ReworkZone(_infernalZones, fireball.dotZonePrefab);

        var miniFireball = fireball.childPrefab.ComponentOrNull<SuperFireballController>();
        if (miniFireball)
            ReworkZone(_infernalZones, miniFireball.dotZonePrefab);
    }

    private static void ReworkBorealZone(GameObject zonePrefab)
    {
        var zone = zonePrefab.ComponentOrNull<ProjectileExpandingDotZone>();
        if (zone)
            _borealZones.Override(() => zone.procCoefficientPerSecond, value => zone.procCoefficientPerSecond = value, _ => 0f);
    }

    private static void ReworkZone(ElitePassiveDamage zones, GameObject zonePrefab)
    {
        var zone = zonePrefab.ComponentOrNull<ProjectileDotZone>();
        if (!zone)
            return;

        zones.SpreadOverOneSecond(zone);
        zones.RemoveProcs(zone);
    }

    private static GameObject ChildProjectile(GameObject projectilePrefab)
    {
        var explosion = projectilePrefab.ComponentOrNull<ProjectileExplosion>();

        return explosion ? explosion.childrenProjectilePrefab : null;
    }
}
