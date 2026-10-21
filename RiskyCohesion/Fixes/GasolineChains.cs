using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using RoR2;

namespace RiskyCohesion.Fixes;

internal static class GasolineChains
{
    private static readonly Queue<Action> _pendingBursts = new();

    private static bool _bursting;

    public static void Install(ConfigFile config)
    {
        var description = new Description("Gasoline bursts set off by another burst's kills go off after it instead of inside it.");
        var queueChainedBursts = new Rule(config.Bind(Sections.Vanilla, "Gasoline Chains", true, description));

        queueChainedBursts.Hook(typeof(GlobalEventManager), nameof(GlobalEventManager.ProcIgniteOnKill), QueueChainedBursts);
    }

    private static void QueueChainedBursts(Action<DamageReport, int, CharacterBody, TeamIndex> orig, DamageReport report, int itemCount, CharacterBody victim, TeamIndex team)
    {
        _pendingBursts.Enqueue(() => orig(report, itemCount, victim, team));
        if (_bursting)
            return;

        _bursting = true;
        try
        {
            while (_pendingBursts.Count > 0)
                _pendingBursts.Dequeue()();
        }
        finally
        {
            _bursting = false;
            _pendingBursts.Clear();
        }
    }
}
