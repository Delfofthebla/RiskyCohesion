using System.Linq;
using BepInEx.Configuration;
using RoR2;

namespace RiskyCohesion.Fixes;

internal static class SandsweptDroneDisplays
{
    private static HurtBox[] _displayHurtBoxes = [];
    private static ConfigEntry<bool> _removeHurtBoxes;

    public static void Install(ConfigFile config)
    {
        _displayHurtBoxes = Sandswept.Main.droneDefs.Where(droneDef => droneDef.displayPrefab).SelectMany(droneDef => droneDef.displayPrefab.GetComponentsInChildren<HurtBox>(true)).ToArray();

        if (_displayHurtBoxes.Length == 0)
            return;

        var description = new Description("Removes the broken hurtbox from Sandswept's Inferno and Voltaic drones on drone vendors.");
        _removeHurtBoxes = config.Bind(ContentMod.Sandswept.DronesSection, "Drone Display Hurtboxes", true, description);
        _removeHurtBoxes.ApplyNowAndOnChange(SyncDisplayHurtBoxes);
    }

    private static void SyncDisplayHurtBoxes()
    {
        foreach (var hurtBox in _displayHurtBoxes)
            hurtBox.gameObject.SetActive(!_removeHurtBoxes.Value);
    }
}
