using System.Runtime.CompilerServices;
using RiskyMod.Enemies;
using RiskyMod.Enemies.DLC1;
using RiskyMod.Items;
using RiskyMod.Items.Common;
using RiskyMod.Items.DLC1.Void;
using RiskyMod.Items.Uncommon;
using RiskyMod.Tweaks.RunScaling;

namespace RiskyCohesion.Sources;

internal static class RiskyModSource
{
    public static readonly string DisableProcChainsSetting = "RiskyMod's Disable Proc Chains";
    public static readonly string CreditMultiplierSetting = "RiskyMod's Combat Director Credit Multiplier";
    public static readonly string NoAllyInfestationSetting = "RiskyMod's Void Infestor - No Ally Infestation";

    public static bool DisablesProcChains { get; private set; }
    public static bool MultipliesDirectorCredits { get; private set; }
    public static float DirectorCreditMultiplier { get; private set; }
    public static bool ChangesGasoline { get; private set; }
    public static bool ChangesUkulele { get; private set; }
    public static bool ChangesSaferSpaces { get; private set; }
    public static bool PreventsAllyInfestation { get; private set; }

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
    }
}
