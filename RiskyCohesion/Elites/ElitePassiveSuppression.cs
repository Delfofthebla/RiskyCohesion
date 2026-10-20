using BepInEx.Configuration;
using RiskyCohesion.Sources;
using RoR2;

namespace RiskyCohesion.Elites;

internal sealed class ElitePassiveSuppression(ConfigEntry<bool> enabled) : Rule(enabled)
{
    public static ElitePassiveSuppression Bind(ConfigFile config, ContentMod contentMod, string eliteName, string passive)
    {
        var description = new Description($"Stuns stop {passive}.")
            .Requires("EliteReworks");

        return new ElitePassiveSuppression(config.BindWhen(EliteReworksSource.IsInstalled, contentMod.ElitesSection, $"{eliteName} Stun", true, description));
    }

    public static bool Suppresses(CharacterBody body) => body && body.HasBuff(EliteReworksSource.PassiveSuppressionBuff);
}
