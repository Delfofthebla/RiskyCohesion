namespace RiskyCohesion.Sources;

internal static class RiskyFixesSource
{
    private static readonly string ItemsSection = "Items";
    private static readonly string FalseSonSection = "Survivors - False Son";
    private static readonly string SaferSpacesCooldownKey = "(Server-Side) Safer Spaces Cooldown";
    private static readonly string BreachingFinReprocKey = "(Server-Side) Breaching Fin - Fix Re-Proc";
    private static readonly string LunarRuinReprocKey = "(Server-Side) Lunar Ruin Exponential Damage Removal";

    public static readonly string SaferSpacesCooldownSetting = "RiskyFixes' Safer Spaces Cooldown";
    public static readonly string LunarRuinReprocSetting = "RiskyFixes' Lunar Ruin Exponential Damage Removal";

    public static bool CorrectsSaferSpacesCooldown { get; private set; }
    public static bool FixesBreachingFinReproc { get; private set; }
    public static bool FixesLunarRuinReproc { get; private set; }

    public static void Load()
    {
        CorrectsSaferSpacesCooldown = IsItemFixOn(SaferSpacesCooldownKey);
        FixesBreachingFinReproc = IsItemFixOn(BreachingFinReprocKey);
        FixesLunarRuinReproc = PluginConfig.IsOn(Mods.RISKY_FIXES, FalseSonSection, LunarRuinReprocKey);
    }

    private static bool IsItemFixOn(string key) => PluginConfig.IsOn(Mods.RISKY_FIXES, ItemsSection, key);
}
