using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using Voidcrid.Modules;

namespace RiskyCohesion.Fixes;

internal static class VoidcridDamageTypes
{
    public static void Install(ConfigFile config)
    {
        var description = new Description("Voidcrid's damage types are registered at startup instead of on the first hit.");
        var registerAtStartup = config.Bind(ContentMod.Voidcrid.SurvivorsSection, "Damage Types", true, description);

        if (registerAtStartup.Value)
            RuntimeHelpers.RunClassConstructor(typeof(DamageTypes).TypeHandle);
    }
}
