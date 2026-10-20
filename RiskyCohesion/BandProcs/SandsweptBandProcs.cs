using BepInEx.Configuration;
using Sandswept.Items;
using Sandswept.Items.Reds;

namespace RiskyCohesion.BandProcs;

internal static class SandsweptBandProcs
{
    public static void Install(ConfigFile config)
    {
        if (ItemBase<GlacialPlasma>.instance?.ItemDef == null)
            return;

        ItemBandProc.Bind(config, ContentMod.Sandswept, "Glacial Plasma", "Glacial Plasma's javelin").RemoveFrom(() => GlacialPlasma.javelinProjectile);
    }
}
