using System;
using System.Collections.Generic;
using ArtificerExtended.Passive;
using BepInEx.Configuration;
using EntityStates.RiskyMod.Mage.Weapon;
using RiskyCohesion.Sources;

namespace RiskyCohesion.Survivors;

internal static class ArtificerExtendedSnapfreeze
{
    private static readonly Dictionary<PrepIceWall, Resonance> _resonances = [];

    public static void Install(ConfigFile config)
    {
        var description = new Description("Casting RiskyMod's reworked Snapfreeze triggers Energetic Resonance.")
            .Requires(RiskyModSource.SnapfreezeReworkSetting);

        var resonate = new Rule(config.BindWhen(RiskyModSource.ReworksSnapfreeze, ContentMod.ArtificerExtended.SurvivorsSection, "Snapfreeze Resonance", true, description));

        resonate.Hook(typeof(PrepIceWall), nameof(PrepIceWall.OnEnter), BeginResonance);
        resonate.Hook(typeof(PrepIceWall), nameof(PrepIceWall.OnExit), ReleaseResonance);
    }

    private static void BeginResonance(Action<PrepIceWall> orig, PrepIceWall prep)
    {
        orig(prep);

        if (!AltArtiPassive.instanceLookup.TryGetValue(prep.gameObject, out var passive))
            return;

        var handle = new AltArtiPassive.BatchHandle();

        passive.SkillCast(handle);
        _resonances[prep] = new Resonance(passive, handle);
    }

    private static void ReleaseResonance(Action<PrepIceWall> orig, PrepIceWall prep)
    {
        orig(prep);

        if (_resonances.Remove(prep, out var resonance))
            resonance.Handle.Fire(resonance.Passive.ext_prepWallMinDelay, resonance.Passive.ext_prepWallMaxDelay);
    }

    private sealed record Resonance(AltArtiPassive Passive, AltArtiPassive.BatchHandle Handle);
}
