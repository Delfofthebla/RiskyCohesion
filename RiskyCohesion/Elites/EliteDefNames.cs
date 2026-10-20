using System.Linq;
using RoR2;

namespace RiskyCohesion.Elites;

internal static class EliteDefNames
{
    public static readonly string Sanguine = "AETHERIUM_ELITE_AFFIX_SANGUINE";
    public static readonly string Toxic = "edPurple";

    public static EliteDef Find(string name) => EliteCatalog.eliteDefs?.FirstOrDefault(eliteDef => eliteDef.name == name);
}
