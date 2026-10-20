using BepInEx.Configuration;
using RoR2;
using SS2.Items;

namespace RiskyCohesion.BandProcs;

internal static class StarstormBandProcs
{
    private static ItemBandProc _hauntedLamp;

    public static void Install(ConfigFile config)
    {
        _hauntedLamp = ItemBandProc.Bind(config, ContentMod.Starstorm, "Haunted Lamp", "Haunted Lamp's projectile");
        if (_hauntedLamp.SourceIsOn)
            RoR2Application.onLoad += HookIfLampLoaded;
    }

    private static void HookIfLampLoaded()
    {
        if (ShackledLamp.projectilePrefab)
            _hauntedLamp.RemoveFrom(() => ShackledLamp.projectilePrefab);
    }
}
