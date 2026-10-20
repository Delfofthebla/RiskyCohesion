using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using RiskyCohesion.Sources;
using RiskyMod.Items;

namespace RiskyCohesion.LookingGlassStats.Vanilla;

internal static class VanillaStats
{
    public static void Install(ConfigFile config)
    {
        var description = new Description("Corrects LookingGlass' stats for the vanilla items RiskyMod, RiskyTweaks and RiskyFixes change.");
        var stats = StatsGroup.Bind(config, "Vanilla Items", description);

        if (Mods.IsLoaded(Mods.RISKY_MOD))
            RegisterRiskyModChanges(stats);

        CommonStats.RegisterGasoline(stats);
        LunarStats.RegisterVisionsOfHeresy(stats);
        VoidStats.RegisterSaferSpaces(stats);
    }

    public static bool RiskyModChanges(bool tierEnabled, bool itemEnabled) => ItemsCore.enabled && tierEnabled && itemEnabled;

    public static float ProcCoefficient(float vanilla) => RiskyModSource.DisablesProcChains ? 0f : vanilla;

    // RiskyMod's types only resolve when RiskyMod is loaded, so they stay out of any method that runs without it.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void RegisterRiskyModChanges(StatsGroup stats)
    {
        CommonStats.Register(stats);
        UncommonStats.Register(stats);
        LegendaryStats.Register(stats);
        BossStats.Register(stats);
        LunarStats.Register(stats);
        VoidStats.Register(stats);
        EquipmentStats.Register(stats);
        FoodStats.Register(stats);
    }
}
