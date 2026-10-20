using LookingGlass.ItemStatsNameSpace;
using RiskyMod.Items;
using RiskyMod.Items.Equipment;
using RoR2;
using Style = LookingGlass.ItemStatsNameSpace.ItemStatsDef.ValueType;
using Unit = LookingGlass.ItemStatsNameSpace.ItemStatsDef.MeasurementUnits;

namespace RiskyCohesion.LookingGlassStats.Vanilla;

internal static class EquipmentStats
{
    public static void Register(StatsGroup stats)
    {
        if (VanillaStats.RiskyModChanges(ItemsCore.equipmentEnabled, Backup.enabled))
            stats.Equipment(() => RoR2Content.Equipment.DroneBackup, TheBackUpStats);
    }

    private static ItemStatsDef TheBackUpStats() => new StatSheet()
        .Line("Max Drones: ", Style.Utility, Unit.Number)
        .PerStack(_ => [BackupTracker.maxCount]);
}
