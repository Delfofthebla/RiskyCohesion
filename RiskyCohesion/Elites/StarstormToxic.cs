using BepInEx.Configuration;
using RiskyCohesion.Sources;
using RoR2;
using RoR2.Projectile;
using SS2.Equipments;

namespace RiskyCohesion.Elites;

internal static class StarstormToxic
{
    private static readonly string Name = "Toxic";

    private static ElitePassiveDamage _bombs;

    public static void Install(ConfigFile config)
    {
        EliteStats.Bind(config, ContentMod.Starstorm, Name, EliteDefNames.Toxic, EliteTier.Tier1, contentModHealth: 4f, contentModDamage: 2f);

        InstallStun(config);
        InstallBombs(config);
    }

    private static void InstallStun(ConfigFile config)
    {
        var stun = ElitePassiveSuppression.Bind(config, ContentMod.Starstorm, Name, "Toxic's poison bombs");

        new StunResetTimer<AffixPurple.AffixPurpleBehavior>(stun, behaviour => behaviour.characterBody, behaviour => behaviour.projectileTimer = 0f).Install();
    }

    private static void InstallBombs(ConfigFile config)
    {
        var description = new Description("Toxic's poison bombs deal Overloading's passive bomb damage, with proc coefficient 0.");

        _bombs = ElitePassiveDamage.Bind(config, ContentMod.Starstorm, Name, EliteRework.OverloadingLightning, description);
        if (!_bombs.SourceIsOn)
            return;

        _bombs.ReplaceProjectileDamage(() => AffixPurple.projectilePrefab, EliteReworksSource.OverloadingBombs);
        RoR2Application.onLoad += () => Failsafe.Run("removing the procs of Toxic's poison bombs", RemoveBombProcs);
    }

    private static void RemoveBombProcs()
    {
        var bomb = AffixPurple.projectilePrefab.ComponentOrNull<ProjectileController>();
        if (bomb)
            _bombs.RemoveProcs(bomb);
    }
}
