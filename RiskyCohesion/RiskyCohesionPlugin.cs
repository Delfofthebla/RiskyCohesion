using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using R2API.Utils;
using RiskyCohesion.Balance;
using RiskyCohesion.BandProcs;
using RiskyCohesion.Elites;
using RiskyCohesion.Fixes;
using RiskyCohesion.LookingGlassStats;
using RiskyCohesion.LookingGlassStats.Vanilla;
using RiskyCohesion.ProcChains;
using RiskyCohesion.Sources;

namespace RiskyCohesion;

[BepInPlugin(GUID, NAME, VERSION)]
[BepInDependency(Mods.RISKY_MOD, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(Mods.RISKY_TWEAKS, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(Mods.RISKY_FIXES, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(Mods.ELITE_REWORKS, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(Mods.AETHERIUM, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(Mods.SANDSWEPT, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(Mods.STARSTORM2, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(Mods.SPIKESTRIP, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(Mods.LOOKING_GLASS, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(RiskOfOptionsMenu.GUID, BepInDependency.DependencyFlags.SoftDependency)]
[NetworkCompatibility(CompatibilityLevel.NoNeedForSync)]
public class RiskyCohesionPlugin : BaseUnityPlugin
{
    public const string GUID = "com.Delfofthebla.RiskyCohesion";
    public const string NAME = "RiskyCohesion";
    public const string VERSION = "1.0.0";

    internal static ManualLogSource Log { get; private set; }

    private bool _hasAetherium;
    private bool _hasSandswept;
    private bool _hasStarstorm;
    private bool _hasSpikestrip;
    private bool _hasLookingGlass;

    private void Awake()
    {
        Log = Logger;

        Hooks.CreateQueuedOnLoad();

        Failsafe.Run("reading RiskyMod's settings", RiskyModSource.Load);
        Failsafe.Run("reading RiskyTweaks' settings", RiskyTweaksSource.Load);
        Failsafe.Run("reading RiskyFixes' settings", RiskyFixesSource.Load);
        Failsafe.Run("reading EliteReworks' settings", EliteReworksSource.Load);

        _hasAetherium = Mods.IsLoaded(Mods.AETHERIUM);
        _hasSandswept = Mods.IsLoaded(Mods.SANDSWEPT);
        _hasStarstorm = Mods.IsLoaded(Mods.STARSTORM2);
        _hasSpikestrip = Mods.IsLoaded(Mods.SPIKESTRIP);
        _hasLookingGlass = Mods.IsLoaded(Mods.LOOKING_GLASS);

        PatchItemStats();
        PatchItems();
        PatchElites();
        PatchSurvivors();
        PatchDrones();
        PatchBalance();
        PatchVanilla();

        RiskOfOptionsMenu.Register(this, Log);
    }

    private void PatchItems()
    {
        if (_hasAetherium)
            Install("disabling Aetherium's proc chains", AetheriumProcChains.Install);

        if (_hasSandswept)
            Install("removing Sandswept's band procs", SandsweptBandProcs.Install);

        if (_hasStarstorm)
        {
            Install("disabling Starstorm 2's proc chains", StarstormProcChains.Install);
            Install("removing Starstorm 2's band procs", StarstormBandProcs.Install);
        }

        if (_hasSpikestrip)
        {
            Install("disabling Spikestrip 2.0's proc chains", SpikestripProcChains.Install);
            Install("fixing Infinity Codex for players other than the host", SpikestripInfinityCodex.Install);
        }
    }

    private void PatchElites()
    {
        if (_hasAetherium)
        {
            Install("reworking Aetherium's elites", AetheriumElites.Install);
            Install("disabling Sanguine's proc chains", AetheriumProcChains.InstallSanguine);
        }

        if (_hasSandswept)
            Install("reworking Sandswept's elites", SandsweptElites.Install);

        if (_hasStarstorm)
        {
            Install("reworking Starstorm 2's elite passive damage", StarstormPassiveDamage.Install);
            Install("suppressing Starstorm 2's elite passives", StarstormPassiveSuppression.Install);
            Install("reworking Starstorm 2's Toxic elite", StarstormToxic.Install);
            Install("matching Starstorm 2's elite director to RiskyMod's credits", StarstormEliteDirector.Install);
            Install("fixing Starstorm 2's elite event health bonus", StarstormEliteEventHealthBonus.Install);
        }

        if (_hasSpikestrip)
        {
            Install("reworking Spikestrip 2.0's elites", SpikestripElites.Install);
            Install("fixing the team of Aragonite's death strikes", SpikestripAragoniteDeathStrikes.Install);
        }
    }

    private void PatchSurvivors()
    {
        if (_hasStarstorm)
            Install("fixing allies infested by Chirr's befriended Void Infestors", StarstormInfestedAllies.Install);
    }

    private void PatchDrones()
    {
        if (_hasSandswept)
            Install("removing Sandswept's drone display hurtboxes", SandsweptDroneDisplays.Install);
    }

    private void PatchBalance()
    {
        if (_hasStarstorm)
        {
            Install("scaling Starstorm 2's late-game elite boosts", StarstormEliteBoosts.Install);
            Install("changing Toxic's poison", StarstormToxicPoison.Install);
        }
    }

    private void PatchVanilla()
    {
        Install("correcting Ukulele's range tooltip", UkuleleRangeTooltip.Install);
        Install("correcting Safer Spaces' recharge tooltip", SaferSpacesRechargeTooltip.Install);
        Install("fixing stuns on the Solus Wing's pods", SolusWingPodStuns.Install);
        Install("fixing Milky Chrysalis wings on bodies without a wings model", MilkyChrysalisWings.Install);
    }

    private void PatchItemStats()
    {
        if (!_hasLookingGlass)
            return;

        Install("fixing LookingGlass' vanilla item stats", LookingGlassVanillaStats.Install);
        Install("correcting LookingGlass' stats for vanilla items", VanillaStats.Install);

        if (_hasAetherium)
            Install("adding LookingGlass stats for Aetherium's items", AetheriumStats.Install);

        if (_hasSandswept)
            Install("adding LookingGlass stats for Sandswept's items", SandsweptStats.Install);

        if (_hasStarstorm)
            Install("adding LookingGlass stats for Starstorm 2's items", StarstormStats.Install);

        if (_hasSpikestrip)
            Install("adding LookingGlass stats for Spikestrip 2.0's items", SpikestripStats.Install);
    }

    private void Install(string task, Action<ConfigFile> install) => Failsafe.Run(task, () => install(Config));
}
