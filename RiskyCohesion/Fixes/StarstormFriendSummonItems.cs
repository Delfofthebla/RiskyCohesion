using System;
using BepInEx.Configuration;
using RoR2;
using SS2.Components;

namespace RiskyCohesion.Fixes;

internal static class StarstormFriendSummonItems
{
    public static void Install(ConfigFile config)
    {
        var description = new Description("Monsters summoned by Chirr's friend copy only the friend's own items, not the ones Chirr shares with it.");
        var copyOwnItems = new Rule(config.Bind(ContentMod.Starstorm.SurvivorsSection, "Friend Summon Items", true, description));

        copyOwnItems.Hook(typeof(MasterSummon), nameof(MasterSummon.Perform), CopyFriendsOwnItems);
    }

    private static CharacterMaster CopyFriendsOwnItems(Func<MasterSummon, CharacterMaster> orig, MasterSummon summon)
    {
        var friendInventory = summon.inventoryToCopy;
        var friendMaster = friendInventory
            ? friendInventory.GetComponent<CharacterMaster>()
            : null;

        var chirr = ChirrFriends.BefriendingChirr(friendMaster);
        if (!chirr)
            return orig(summon);

        var copyFilter = summon.inventoryItemCopyFilter ?? Inventory.defaultItemCopyFilterDelegate;
        var inventorySetup = summon.inventorySetupCallback;

        summon.inventoryToCopy = null;
        summon.inventorySetupCallback = new FriendsOwnItems(chirr.currentFriend, copyFilter, inventorySetup);

        try
        {
            return orig(summon);
        }
        finally
        {
            summon.inventoryToCopy = friendInventory;
            summon.inventorySetupCallback = inventorySetup;
        }
    }

    private sealed class FriendsOwnItems(ChirrFriendController.Friend friend, Func<ItemIndex, bool> copyFilter, MasterSummon.IInventorySetupCallback next) : MasterSummon.IInventorySetupCallback
    {
        public void SetupSummonedInventory(MasterSummon summon, Inventory summoned)
        {
            summoned.InventoryChangedHandler(CopyOwnItems);

            next?.SetupSummonedInventory(summon, summoned);
        }

        private void CopyOwnItems(Inventory summoned)
        {
            summoned.CopyEquipmentFrom(friend.master.inventory, includeChargeData: false);

            for (var index = 0; index < friend.permanentItemStacks.Length; index++)
            {
                var item = (ItemIndex)index;
                if (!copyFilter(item))
                    continue;

                if (friend.permanentItemStacks[index] > 0)
                    summoned.GiveItemPermanent(item, friend.permanentItemStacks[index]);

                if (friend.tempItemTimers[index] > 0)
                    summoned.GiveItemTemp(item, friend.tempItemTimers[index]);
            }
        }
    }
}
