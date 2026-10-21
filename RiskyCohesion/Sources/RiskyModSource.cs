using System.Runtime.CompilerServices;
using RiskyMod.Enemies;
using RiskyMod.Enemies.DLC1;
using RiskyMod.Items;
using RiskyMod.Items.Common;
using RiskyMod.Items.DLC1.Void;
using RiskyMod.Items.Uncommon;
using RiskyMod.Survivors.Bandit2;
using RiskyMod.Survivors.Captain;
using RiskyMod.Survivors.Commando;
using RiskyMod.Survivors.Croco;
using RiskyMod.Survivors.Croco.Tweaks;
using RiskyMod.Survivors.Huntress;
using RiskyMod.Survivors.Loader;
using RiskyMod.Survivors.Mage;
using RiskyMod.Survivors.Treebot;
using RiskyMod.Tweaks.CharacterMechanics;
using RiskyMod.Tweaks.RunScaling;

namespace RiskyCohesion.Sources;

internal static class RiskyModSource
{
    public static readonly string DisableProcChainsSetting = "RiskyMod's Disable Proc Chains";
    public static readonly string CreditMultiplierSetting = "RiskyMod's Combat Director Credit Multiplier";
    public static readonly string NoAllyInfestationSetting = "RiskyMod's Void Infestor - No Ally Infestation";
    public static readonly string IonSurgeShockSetting = "RiskyMod's Ion Surge - Shock";
    public static readonly string SnapfreezeReworkSetting = "RiskyMod's Snapfreeze Rework";
    public static readonly string SuppressiveFireSetting = "RiskyMod's Suppressive Fire Changes";
    public static readonly string BallistaSetting = "RiskyMod's Ballista Changes";
    public static readonly string ThunderGauntletSetting = "RiskyMod's Thunder Gauntlet Changes";
    public static readonly string ArtificerSetting = "RiskyMod's Artificer Enable Changes";
    public static readonly string DiabloStrikeSetting = "RiskyMod's Diablo Strike Proc Coeficient, Remove Falloff or Disable Friendly Fire";
    public static readonly string HarvestSetting = "RiskyMod's DIRECTIVE Harvest Changes";
    public static readonly string BanditSpecialReworkSetting = "RiskyMod's Bandit Special Rework";
    public static readonly string FreezeExecutesBossesSetting = "RiskyMod's Freeze Executes Bosses";
    public static readonly string CloyingBreathNerfSetting = "RiskyMod's Contagion Passive Skill or Blight Returns";

    public static bool DisablesProcChains { get; private set; }
    public static bool MultipliesDirectorCredits { get; private set; }
    public static float DirectorCreditMultiplier { get; private set; }
    public static bool ChangesGasoline { get; private set; }
    public static bool ChangesUkulele { get; private set; }
    public static bool ChangesSaferSpaces { get; private set; }
    public static bool PreventsAllyInfestation { get; private set; }
    public static bool ShocksWithIonSurge { get; private set; }
    public static bool ReworksSnapfreeze { get; private set; }
    public static bool ChangesSuppressiveFire { get; private set; }
    public static bool ChangesBallista { get; private set; }
    public static bool ChangesThunderGauntlet { get; private set; }
    public static bool ChangesArtificer { get; private set; }
    public static bool ChangesDiabloStrike { get; private set; }
    public static bool ChangesHarvest { get; private set; }
    public static bool ReworksBanditSpecials { get; private set; }
    public static bool FreezeExecutesBosses { get; private set; }
    public static bool NerfsCloyingBreath { get; private set; }

    public static void Load()
    {
        if (Mods.IsLoaded(Mods.RISKY_MOD))
            ReadSettings();
    }

    // RiskyMod's types only resolve when RiskyMod is loaded, so they stay out of any method that runs without it.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ReadSettings()
    {
        DisablesProcChains = ItemsCore.enabled && RiskyMod.RiskyMod.disableProcChains;
        MultipliesDirectorCredits = CombatDirectorMultiplier.enabled;
        DirectorCreditMultiplier = CombatDirectorMultiplier.directorCreditMultiplier;
        ChangesGasoline = ItemsCore.enabled && ItemsCore.commonEnabled && Gasoline.enabled;
        ChangesUkulele = ItemsCore.enabled && ItemsCore.uncommonEnabled && Ukulele.enabled;
        ChangesSaferSpaces = ItemsCore.enabled && ItemsCore.voidEnabled && SaferSpaces.enabled;
        PreventsAllyInfestation = EnemiesCore.modifyEnemies && VoidInfestor.enabled && VoidInfestor.noVoidAllies;
        ShocksWithIonSurge = MageCore.enabled && MageCore.ionSurgeShock;
        ReworksSnapfreeze = MageCore.enabled && MageCore.iceWallRework;
        ChangesSuppressiveFire = CommandoCore.enabled && CommandoCore.suppressiveChanges;
        ChangesBallista = HuntressCore.enabled && HuntressCore.ballistaChanges;
        ChangesThunderGauntlet = LoaderCore.enabled && LoaderCore.zapFistChanges;
        ChangesArtificer = MageCore.enabled;
        ChangesDiabloStrike = CaptainCore.enabled && (CaptainCore.nukeProc || CaptainCore.nukeFalloff || CaptainCore.disableNukeFriendlyFire);
        ChangesHarvest = TreebotCore.enabled && TreebotCore.fruitChanges;
        ReworksBanditSpecials = Bandit2Core.enabled && Bandit2Core.specialRework;
        FreezeExecutesBosses = FreezeChampionExecute.enabled;
        NerfsCloyingBreath = CrocoCore.enabled && NerfCloyingBreath.enabled;
    }
}
