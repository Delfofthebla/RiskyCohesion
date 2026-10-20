using System;
using BepInEx.Configuration;
using RoR2;
using UnityEngine.Networking;

namespace RiskyCohesion.Fixes;

internal static class MilkyChrysalisWings
{
    public static void Install(ConfigFile config)
    {
        var description = new Description("Wings on bodies without a Milky Chrysalis wings model, like monsters in Simulacrum's Flight waves, come off without cutting their death short.");
        var releaseWithoutModel = new Rule(config.Bind(Sections.Vanilla, "Milky Chrysalis Wings", true, description));

        releaseWithoutModel.Hook(typeof(JetpackController), nameof(JetpackController.ReleaseBody), ReleaseWithoutWingModel);
    }

    private static void ReleaseWithoutWingModel(Action<JetpackController> orig, JetpackController self)
    {
        if (self.wingAnimator)
            orig(self);
        else if (NetworkServer.active)
            UnityEngine.Object.Destroy(self.gameObject);
    }
}
