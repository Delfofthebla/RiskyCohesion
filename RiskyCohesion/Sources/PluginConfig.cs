using BepInEx.Bootstrap;

namespace RiskyCohesion.Sources;

internal static class PluginConfig
{
    public static bool IsOn(string guid, string section, string key)
        => Chainloader.PluginInfos.TryGetValue(guid, out var plugin) && plugin.Instance && plugin.Instance.Config.TryGetEntry<bool>(section, key, out var entry) && entry.Value;
}
