using BepInEx.Configuration;

namespace RiskyCohesion;

internal static class ConfigFileExtensions
{
    public static ConfigEntry<T> BindWhen<T>(this ConfigFile config, bool sourceIsOn, string section, string key, T defaultValue, string description)
        => config.BindWhen(sourceIsOn, section, key, defaultValue, new ConfigDescription(description));

    public static ConfigEntry<T> BindWhen<T>(this ConfigFile config, bool sourceIsOn, string section, string key, T defaultValue, ConfigDescription description) => sourceIsOn
        ? config.Bind(section, key, defaultValue, description)
        : null;
}
