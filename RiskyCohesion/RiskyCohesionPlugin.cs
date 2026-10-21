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
using RiskyCohesion.Survivors;

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
[BepInDependency(Mods.SIMULACRUM_ADDITIONS, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(Mods.ENEMY_ABILITIES, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(Mods.ARTIFICER_EXTENDED, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(Mods.ANCIENT_SCEPTER, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(Mods.STORMBOUND_FALSE_SON, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(Mods.CHEF_OVERCOOKED, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(Mods.EGGS_SKILLS, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(Mods.VOIDCRID, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(Mods.LINEAR_DAMAGE, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(Mods.R2API_EXECUTE, BepInDependency.DependencyFlags.SoftDependency)]
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
    private bool _hasSimulacrumAdditions;
    private bool _hasEnemyAbilities;
    private bool _hasArtificerExtended;
    private bool _hasAncientScepter;
    private bool _hasStormboundFalseSon;
    private bool _hasChefOvercooked;
    private bool _hasEggsSkills;
    private bool _hasVoidcrid;
    private bool _hasExecuteApi;
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
        _hasSimulacrumAdditions = Mods.IsLoaded(Mods.SIMULACRUM_ADDITIONS);
        _hasEnemyAbilities = Mods.IsLoaded(Mods.ENEMY_ABILITIES);
        _hasArtificerExtended = Mods.IsLoaded(Mods.ARTIFICER_EXTENDED);
        _hasAncientScepter = Mods.IsLoaded(Mods.ANCIENT_SCEPTER);
        _hasStormboundFalseSon = Mods.IsLoaded(Mods.STORMBOUND_FALSE_SON);
        _hasChefOvercooked = Mods.IsLoaded(Mods.CHEF_OVERCOOKED);
        _hasEggsSkills = Mods.IsLoaded(Mods.EGGS_SKILLS);
        _hasVoidcrid = Mods.IsLoaded(Mods.VOIDCRID);
        _hasExecuteApi = Mods.IsLoaded(Mods.R2API_EXECUTE);
        _hasLookingGlass = Mods.IsLoaded(Mods.LOOKING_GLASS);

        PatchItemStats();
        PatchItems();
        PatchElites();
        PatchSurvivors();
        PatchDrones();
        PatchWaves();
        PatchArtifacts();
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
        {
            Install("fixing allies infested by Chirr's befriended Void Infestors", StarstormInfestedAllies.Install);
            Install("keeping Chirr's shared items off summons by Chirr's friend", StarstormFriendSummonItems.Install);

            if (RiskyTweaksSource.AutofiresBanditPrimaries)
                Install("keeping Tranquilizer Gun's fire rate under RiskyTweaks' autofire", BanditAutofire.InstallTranquilizerGun);
        }

        if (_hasSpikestrip)
        {
            if (RiskyModSource.NerfsCloyingBreath)
                Install("nerfing Deeprot's Cloying Breath like RiskyMod's Blight", SpikestripSkills.InstallDeeprot);

            Install("keeping sprint on Conduit", SpikestripSkills.InstallConduit);
        }

        if (_hasArtificerExtended)
        {
            Install("shocking with ArtificerExtended's Ion Surge", ArtificerExtendedIonSurgeShock.Install);
            Install("holding ArtificerExtended's Ion Surge to a fixed speed", ArtificerExtendedIonSurgeSpeed.Install);
            Install("triggering Energetic Resonance from RiskyMod's Snapfreeze", ArtificerExtendedSnapfreeze.Install);
        }

        if (_hasAncientScepter)
            Install("carrying the Risky mods' skill changes over to Ancient Scepter's upgrades", AncientScepterSkills.Install);

        if (_hasStormboundFalseSon && RiskyFixesSource.FixesLunarRuinReproc)
            Install("capping RiskyFixes' Lunar Ruin bonus at StormboundFalseSon's cap", LunarRuinBonus.InstallStormboundCap);

        if (_hasChefOvercooked && _hasExecuteApi)
            Install("restoring Cook's execute", ChefOvercookedCook.Install);

        if (_hasEggsSkills && RiskyTweaksSource.AutofiresBanditPrimaries)
            Install("keeping Magic Bullet's fire rate under RiskyTweaks' autofire", BanditAutofire.InstallMagicBullet);

        if (_hasVoidcrid)
            Install("registering Voidcrid's damage types at startup", VoidcridDamageTypes.Install);
    }

    private void PatchDrones()
    {
        if (_hasSandswept)
            Install("removing Sandswept's drone display hurtboxes", SandsweptDroneDisplays.Install);
    }

    private void PatchWaves()
    {
        if (_hasSimulacrumAdditions)
            Install("limiting Simulacrum Additions' family waves to stages they can spawn on", SimulacrumAdditionsFamilyWaves.Install);
    }

    private void PatchArtifacts()
    {
        if (_hasStarstorm && _hasEnemyAbilities)
            Install("keeping EnemyAbilities' summon depth on Cognation ghosts", StarstormCognationBeetleSummons.Install);
    }

    private void PatchBalance()
    {
        if (_hasStarstorm)
        {
            Install("scaling Starstorm 2's late-game elite boosts", StarstormEliteBoosts.Install);
            Install("changing Toxic's poison", StarstormToxicPoison.Install);
            Install("raising Chirr's friend limit with a Lysate Cell", StarstormLysateFriend.Install);
        }
    }

    private void PatchVanilla()
    {
        Install("correcting Ukulele's range tooltip", UkuleleRangeTooltip.Install);
        Install("correcting Safer Spaces' recharge tooltip", SaferSpacesRechargeTooltip.Install);
        Install("fixing stuns on the Solus Wing's pods", SolusWingPodStuns.Install);
        Install("fixing Milky Chrysalis wings on bodies without a wings model", MilkyChrysalisWings.Install);
        Install("skipping death stats of unregistered bodies", UnregisteredBodyDeaths.Install);
        Install("queueing Gasoline bursts set off by another burst", GasolineChains.Install);

        if (RiskyFixesSource.FixesLunarRuinReproc)
            Install("fixing RiskyFixes' Lunar Ruin bonus", LunarRuinBonus.InstallFixes);
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
