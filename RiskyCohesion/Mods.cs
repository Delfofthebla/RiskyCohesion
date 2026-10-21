using BepInEx.Bootstrap;

namespace RiskyCohesion;

internal static class Mods
{
    public const string RISKY_MOD = "com.RiskyLives.RiskyMod";
    public const string RISKY_TWEAKS = "com.Moffein.RiskyTweaks";
    public const string RISKY_FIXES = "com.Moffein.RiskyFixes";
    public const string ELITE_REWORKS = "com.Moffein.EliteReworks2";
    public const string AETHERIUM = "com.KomradeSpectre.Aetherium";
    public const string SANDSWEPT = "com.TeamSandswept.Sandswept";
    public const string STARSTORM2 = "com.TeamMoonstorm.Starstorm2";
    public const string SPIKESTRIP = "com.groovesalad.GrooveSaladSpikestripContent";
    public const string SIMULACRUM_ADDITIONS = "Wolfo.SimulacrumAdditions";
    public const string ENEMY_ABILITIES = "Skeletogne.EnemyAbilities";
    public const string ARTIFICER_EXTENDED = "com.Borbo.ArtificerExtended";
    public const string ANCIENT_SCEPTER = "com.DestroyedClone.AncientScepter";
    public const string STORMBOUND_FALSE_SON = "com.Bloonjitsu7.StormboundFalseSon";
    public const string CHEF_OVERCOOKED = "noodleGemo.Chef_Overcooked";
    public const string EGGS_SKILLS = "com.Egg.EggsSkills";
    public const string VOIDCRID = "com.sixfears7.Voidcrid";
    public const string LINEAR_DAMAGE = "com.RiskyLives.LinearDamage";
    public const string R2API_EXECUTE = "com.bepis.r2api.execute";
    public const string LOOKING_GLASS = "droppod.lookingglass";
    public const string HERETIC = "com.Moffein.Heretic";

    public static bool IsLoaded(string guid) => Chainloader.PluginInfos.ContainsKey(guid);
}
