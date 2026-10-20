using System;
using BepInEx.Configuration;
using RoR2;

namespace RiskyCohesion.Fixes;

internal static class SolusWingPodStuns
{
    private static readonly string[] StunStateMethods =
    [
        nameof(SetStateOnHurt.SetStunInternal),
        nameof(SetStateOnHurt.OverrideStunInternal),
        nameof(SetStateOnHurt.SetFrozenInternal),
        nameof(SetStateOnHurt.SetMidasTouchFrozenInternal),
        nameof(SetStateOnHurt.SetShockInternal),
    ];

    public static void Install(ConfigFile config)
    {
        var description = new Description("The Solus Wing's pods ignore stuns, freezes and shocks instead of throwing errors.");
        var ignoreStuns = new Rule(config.Bind(Sections.Vanilla, "Solus Wing Pod Stuns", true, description));

        foreach (var method in StunStateMethods)
            ignoreStuns.Hook(typeof(SetStateOnHurt), method, SkipWhileDisabled);
    }

    private static void SkipWhileDisabled(Action<SetStateOnHurt, float> orig, SetStateOnHurt self, float duration)
    {
        if (self.enabled)
            orig(self, duration);
    }
}
