namespace RiskyCohesion.Sources;

internal static class RiskyTweaksSource
{
    private static readonly string ItemsSection = "Items";
    private static readonly string ShurikenNoBandProcKey = "(Server-Side) Shuriken - No Band Proc";
    private static readonly string GasolineFasterBurnKey = "(Server-Side) Gasoline - Faster Burn Damage";
    private static readonly string VisionsAttackSpeedKey = "(Client-Side) Visions of Heresy - Attack Speed Scaling";

    public static readonly string ShurikenNoBandProcSetting = "RiskyTweaks' Shuriken - No Band Proc";

    public static bool ShurikenSkipsBands { get; private set; }
    public static bool ShurikenSkipsBandsOnlyForPlayers { get; private set; }
    public static bool GasolineBurnsFaster { get; private set; }
    public static bool VisionsScaleWithAttackSpeed { get; private set; }

    public static void Load()
    {
        ShurikenSkipsBands = IsItemTweakOn(ShurikenNoBandProcKey);
        ShurikenSkipsBandsOnlyForPlayers = IsItemTweakOn($"{ShurikenNoBandProcKey} - Only Affect Players");
        GasolineBurnsFaster = IsItemTweakOn(GasolineFasterBurnKey);

        // RiskyTweaks skips this tweak when Heretic is loaded, and soft-depends on Heretic, so Heretic has loaded before this runs.
        VisionsScaleWithAttackSpeed = IsItemTweakOn(VisionsAttackSpeedKey) && !Mods.IsLoaded(Mods.HERETIC);
    }

    private static bool IsItemTweakOn(string key) => PluginConfig.IsOn(Mods.RISKY_TWEAKS, ItemsSection, key);
}
