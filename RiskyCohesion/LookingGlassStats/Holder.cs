using System;
using LookingGlass.ItemStatsNameSpace;
using RoR2;

namespace RiskyCohesion.LookingGlassStats;

internal static class Holder
{
    public static float LevelHealth(CharacterBody body) => body.baseMaxHealth + body.levelMaxHealth * (body.level - 1f);

    public static float RegenMultiplier(CharacterMaster master) => RegenMultiplier(master ? master.inventory : null);

    public static float RegenMultiplier(CharacterBody body) => RegenMultiplier(body ? body.inventory : null);

    public static int ItemCount(CharacterMaster master, ItemDef item) => master && master.inventory
        ? master.inventory.GetItemCountEffective(item)
        : 0;

    public static int PlayerTeamItemCount(CharacterMaster holder, ItemDef item, int stack) => ItemCountWithHolderAt(holder, item, stack, IsOnPlayerTeam);

    public static int LivingItemCount(CharacterMaster holder, ItemDef item, int stack) => ItemCountWithHolderAt(holder, item, stack, IsAliveAndConnected);

    private static float RegenMultiplier(Inventory inventory) => inventory && Run.instance
        ? ItemDefinitions.GetRegenMult(inventory)
        : 1f;

    private static int ItemCountWithHolderAt(CharacterMaster holder, ItemDef item, int stack, Func<CharacterMaster, bool> isCounted)
    {
        var count = stack;
        foreach (var master in CharacterMaster.readOnlyInstancesList)
        {
            if (master != holder && isCounted(master))
                count += ItemCount(master, item);
        }

        return count;
    }

    private static bool IsOnPlayerTeam(CharacterMaster master) => master.teamIndex == TeamIndex.Player && IsConnected(master);

    private static bool IsAliveAndConnected(CharacterMaster master) => master.hasBody && IsConnected(master);

    private static bool IsConnected(CharacterMaster master) => !master.playerCharacterMasterController || master.playerCharacterMasterController.isConnected;
}
