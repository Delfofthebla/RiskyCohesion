using BepInEx.Configuration;
using GrooveSaladSpikestripContent.Content;
using RoR2.Projectile;
using Spikestrip2_0.Core;

namespace RiskyCohesion.ProcChains;

internal static class SpikestripProcChains
{
    public static void Install(ConfigFile config)
    {
        if (SpikestripContent.IsEnabled<PermaSpike>())
            OverridePermaSpike(config);

        if (SpikestripContent.IsEnabled<DotProcs>())
            SpikestripBloodOrange.Install(config);
    }

    private static void OverridePermaSpike(ConfigFile config)
    {
        var procChains = ProcChainGroup.Bind(config, ContentMod.Spikestrip);
        var spikePile = PermaSpike.projectilePrefab.GetComponent<ProjectileController>();

        procChains.Override("Perma-Spike Factory", "Perma-Spike Factory's spike piles", spikePile.procCoefficient, value => spikePile.procCoefficient = value);
    }
}
