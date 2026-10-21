namespace RiskyCohesion.Sources;

internal static class RiskyTweaksSource
{
    private static readonly string ItemsSection = "Items";
    private static readonly string ArtificerSection = "Survivors - Artificer";
    private static readonly string LoaderSection = "Survivors - Loader";
    private static readonly string VoidFiendSection = "Survivors - Void Fiend";
    private static readonly string BanditSection = "Survivors - Bandit";
    private static readonly string ShurikenNoBandProcKey = "(Server-Side) Shuriken - No Band Proc";
    private static readonly string GasolineFasterBurnKey = "(Server-Side) Gasoline - Faster Burn Damage";
    private static readonly string VisionsAttackSpeedKey = "(Client-Side) Visions of Heresy - Attack Speed Scaling";
    private static readonly string IonSurgeMoveSpeedKey = "(Client-Side) Ion Surge - Disable Movement Speed Scaling";
    private static readonly string LoaderSprintKey = "(Client-Side) Sprint QoL";
    private static readonly string CorruptedCrushKey = "(Client-Side) Corrupted Crush - Buff Skill";
    private static readonly string BanditPrimaryAutofireKey = "(Client-Side) Primary Autofire";

    public static readonly string ShurikenNoBandProcSetting = "RiskyTweaks' Shuriken - No Band Proc";
    public static readonly string IonSurgeMoveSpeedSetting = "RiskyTweaks' Ion Surge - Disable Movement Speed Scaling";
    public static readonly string LoaderSprintSetting = "RiskyTweaks' Loader Sprint QoL";
    public static readonly string CorruptedCrushSetting = "RiskyTweaks' Corrupted Crush - Buff Skill";
    public static readonly string BanditPrimaryAutofireSetting = "RiskyTweaks' Bandit Primary Autofire";

    public static bool ShurikenSkipsBands { get; private set; }
    public static bool ShurikenSkipsBandsOnlyForPlayers { get; private set; }
    public static bool GasolineBurnsFaster { get; private set; }
    public static bool VisionsScaleWithAttackSpeed { get; private set; }
    public static bool IonSurgeIgnoresMoveSpeed { get; private set; }
    public static bool LoaderSkillsKeepSprint { get; private set; }
    public static bool BuffsCorruptedCrush { get; private set; }
    public static bool AutofiresBanditPrimaries { get; private set; }

    public static void Load()
    {
        ShurikenSkipsBands = IsItemTweakOn(ShurikenNoBandProcKey);
        ShurikenSkipsBandsOnlyForPlayers = IsItemTweakOn($"{ShurikenNoBandProcKey} - Only Affect Players");
        GasolineBurnsFaster = IsItemTweakOn(GasolineFasterBurnKey);

        // RiskyTweaks skips this tweak when Heretic is loaded, and soft-depends on Heretic, so Heretic has loaded before this runs.
        VisionsScaleWithAttackSpeed = IsItemTweakOn(VisionsAttackSpeedKey) && !Mods.IsLoaded(Mods.HERETIC);

        IonSurgeIgnoresMoveSpeed = PluginConfig.IsOn(Mods.RISKY_TWEAKS, ArtificerSection, IonSurgeMoveSpeedKey);
        LoaderSkillsKeepSprint = PluginConfig.IsOn(Mods.RISKY_TWEAKS, LoaderSection, LoaderSprintKey);
        BuffsCorruptedCrush = PluginConfig.IsOn(Mods.RISKY_TWEAKS, VoidFiendSection, CorruptedCrushKey);
        AutofiresBanditPrimaries = PluginConfig.IsOn(Mods.RISKY_TWEAKS, BanditSection, BanditPrimaryAutofireKey);
    }

    private static bool IsItemTweakOn(string key) => PluginConfig.IsOn(Mods.RISKY_TWEAKS, ItemsSection, key);
}
