using System;
using BepInEx.Configuration;
using RoR2;
using RoR2.Stats;

namespace RiskyCohesion.Fixes;

internal static class UnregisteredBodyDeaths
{
    public static void Install(ConfigFile config)
    {
        var description = new Description("Stat tracking skips deaths of removed bodies missing from the body catalog instead of throwing an error that cuts the frame short.");
        var skipUnidentified = new Rule(config.Bind(Sections.Vanilla, "Unregistered Body Deaths", true, description));

        skipUnidentified.Hook(typeof(StatManager), nameof(StatManager.ProcessDeathEvents), DropUnidentifiedDeaths);
    }

    private static void DropUnidentifiedDeaths(Action orig)
    {
        var deathEvents = StatManager.deathEvents;
        for (var remaining = deathEvents.Count; remaining > 0; remaining--)
        {
            var deathEvent = deathEvents.Dequeue();
            if (CanIdentifyVictim(deathEvent.damageReport))
                deathEvents.Enqueue(deathEvent);
        }

        orig();
    }

    private static bool CanIdentifyVictim(DamageReport damageReport) => damageReport.victimBody || BodyCatalog.GetBodyPrefab(damageReport.victimBodyIndex);
}
