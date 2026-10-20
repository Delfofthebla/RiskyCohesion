namespace RiskyCohesion.Sources;

internal static class RiskyFixesSource
{
    private static readonly string ItemsSection = "Items";
    private static readonly string SaferSpacesCooldownKey = "(Server-Side) Safer Spaces Cooldown";
    private static readonly string BreachingFinReprocKey = "(Server-Side) Breaching Fin - Fix Re-Proc";

    public static readonly string SaferSpacesCooldownSetting = "RiskyFixes' Safer Spaces Cooldown";

    public static bool CorrectsSaferSpacesCooldown { get; private set; }
    public static bool FixesBreachingFinReproc { get; private set; }

    public static void Load()
    {
        CorrectsSaferSpacesCooldown = IsItemFixOn(SaferSpacesCooldownKey);
        FixesBreachingFinReproc = IsItemFixOn(BreachingFinReprocKey);
    }

    private static bool IsItemFixOn(string key) => PluginConfig.IsOn(Mods.RISKY_FIXES, ItemsSection, key);
}
