using BepInEx.Configuration;
using RoR2;

namespace RiskyCohesion.Elites;

internal sealed class EliteStats(ConfigEntry<float> health, ConfigEntry<float> damage, string eliteDefName)
{
    private EliteDef _eliteDef;

    public static void Bind(ConfigFile config, ContentMod contentMod, string eliteName, string eliteDefName, EliteTier tier, float contentModHealth, float contentModDamage)
    {
        if (!tier.Rework.IsOn)
            return;

        var health = BindMultiplier(config, contentMod, eliteName, tier, "Health", contentModHealth, tier.Health);
        var damage = BindMultiplier(config, contentMod, eliteName, tier, "Damage", contentModDamage, tier.Damage);
        var stats = new EliteStats(health, damage, eliteDefName);

        RoR2Application.onLoad += () => Failsafe.Run($"setting {eliteName}'s stats", stats.Capture);
    }

    private static ConfigEntry<float> BindMultiplier(ConfigFile config, ContentMod contentMod, string eliteName, EliteTier tier, string stat, float contentModValue, TierMultiplier tierValue)
    {
        var description = new Description($"{eliteName}'s {stat.ToLowerInvariant()} multiplier.")
            .Requires(tier.Rework.Requirement)
            .Reference(contentMod.Name, Description.Format(contentModValue))
            .Reference("EliteReworks", $"{Description.Format(tierValue.Reworked)} ({tier.Name} elites)")
            .Vanilla($"{Description.Format(tierValue.Vanilla)} ({tier.Name} elites)");

        return config.Bind(contentMod.ElitesSection, $"{eliteName} {stat}", tierValue.Reworked, new ConfigDescription(description, new AcceptableValueRange<float>(1f, tierValue.Vanilla * 2f)));
    }

    private void Capture()
    {
        _eliteDef = EliteDefNames.Find(eliteDefName);
        if (!_eliteDef)
            return;

        Apply();
        health.SettingChanged += (_, _) => Apply();
        damage.SettingChanged += (_, _) => Apply();
    }

    private void Apply()
    {
        _eliteDef.healthBoostCoefficient = health.Value;
        _eliteDef.damageBoostCoefficient = damage.Value;
    }
}
