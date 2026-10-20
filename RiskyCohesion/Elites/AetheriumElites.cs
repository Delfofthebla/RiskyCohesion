using Aetherium.EliteEquipment;
using BepInEx.Configuration;

namespace RiskyCohesion.Elites;

internal static class AetheriumElites
{
    public static bool SanguineIsEnabled { get; } = EliteEquipmentBase<AffixSanguine>.instance?.EliteEquipmentDef != null;

    public static void Install(ConfigFile config)
    {
        if (!SanguineIsEnabled)
            return;

        EliteStats.Bind(config, ContentMod.Aetherium, "Sanguine", EliteDefNames.Sanguine, EliteTier.Tier1, contentModHealth: 4f, contentModDamage: 2f);
    }
}
