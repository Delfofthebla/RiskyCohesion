using System;
using BepInEx.Configuration;
using RiskyCohesion.Sources;
using RoR2;
using SS2.Components;

namespace RiskyCohesion.Elites;

internal static class StarstormEliteDirector
{
    public static void Install(ConfigFile config)
    {
        var description = new Description("Starstorm 2's elite director ignores the credit multiplier.")
            .Requires(RiskyModSource.CreditMultiplierSetting);

        var matchRiskyModCredits = new Rule(config.BindWhen(RiskyModSource.MultipliesDirectorCredits, ContentMod.Starstorm.ElitesSection, "Late-Game Elite Spawns", true, description));

        matchRiskyModCredits.Hook(typeof(CustomEliteDirector), nameof(CustomEliteDirector.ModifySpawn), ModifySpawnWithDesignedCredits);
    }

    private static void ModifySpawnWithDesignedCredits(
        Action<CustomEliteDirector, CombatDirector, SpawnCard.SpawnResult> orig,
        CustomEliteDirector self,
        CombatDirector director,
        SpawnCard.SpawnResult spawnResult)
    {
        if (!HasMultipliedCredits(director))
        {
            orig(self, director, spawnResult);
            return;
        }

        var multiplier = RiskyModSource.DirectorCreditMultiplier;
        director.monsterCredit /= multiplier;

        try
        {
            orig(self, director, spawnResult);
        }
        finally
        {
            director.monsterCredit *= multiplier;
        }
    }

    private static bool HasMultipliedCredits(CombatDirector director)
    {
        if (!director || Run.instance is InfiniteTowerRun)
            return false;

        var teleporter = TeleporterInteraction.instance;

        return !teleporter || director != teleporter.bossDirector;
    }
}
