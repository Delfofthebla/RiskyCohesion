using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using RoR2;
using SS2.Components;

namespace RiskyCohesion.Balance;

internal static class StarstormLysateFriend
{
    private static readonly ConditionalWeakTable<CharacterMaster, List<ChirrFriendController>> _befriendOrder = new();

    public static void Install(ConfigFile config)
    {
        var description = new Description("A Lysate Cell raises Chirr's friend limit to two, and befriending past the limit releases the oldest friend.");
        var lysateFriend = new Rule(config.Bind(Sections.Balance, "Chirr Lysate Cell Friend", false, description));

        lysateFriend.Hook(typeof(ChirrFriendController), nameof(ChirrFriendController.AddFriend), AddWithinLimit);
        lysateFriend.Hook(typeof(ChirrFriendController), nameof(ChirrFriendController.UpdateScepter), UpdateSpareScepters);
    }

    private static void AddWithinLimit(Action<ChirrFriendController, CharacterMaster> orig, ChirrFriendController self, CharacterMaster friend)
    {
        if (!IsPrimary(self))
        {
            Befriend(orig, self, friend);
            return;
        }

        var slots = self.GetComponents<ChirrFriendController>();
        var slot = slots.FirstOrDefault(candidate => candidate.currentFriend.master == friend) ?? FreeSlot(self, slots);

        if (slot == self)
            Befriend(orig, self, friend);
        else
            slot.AddFriend(friend);
    }

    private static void UpdateSpareScepters(Action<ChirrFriendController> orig, ChirrFriendController self)
    {
        orig(self);

        if (!IsPrimary(self))
            return;

        foreach (var spare in self.GetComponents<ChirrFriendController>().Skip(1))
            spare.isScepter = self.isScepter;
    }

    private static void Befriend(Action<ChirrFriendController, CharacterMaster> orig, ChirrFriendController slot, CharacterMaster friend)
    {
        orig(slot, friend);

        var order = _befriendOrder.GetOrCreateValue(slot.master);
        order.Remove(slot);
        order.Add(slot);
    }

    private static ChirrFriendController FreeSlot(ChirrFriendController primary, ChirrFriendController[] slots)
    {
        ReleaseOldestBeyond(FriendLimit(primary.master) - 1, slots, _befriendOrder.GetOrCreateValue(primary.master));

        return slots.FirstOrDefault(slot => !slot.hasFriend) ?? AddSpareSlot(primary);
    }

    private static void ReleaseOldestBeyond(int kept, ChirrFriendController[] slots, List<ChirrFriendController> order)
    {
        var oldestFirst = slots.Where(slot => slot.hasFriend).OrderBy(order.IndexOf).ToList();

        foreach (var slot in oldestFirst.Take(oldestFirst.Count - kept))
            slot.RemoveFriend(slot.currentFriend.master);
    }

    private static ChirrFriendController AddSpareSlot(ChirrFriendController primary)
    {
        var spare = primary.gameObject.AddComponent<ChirrFriendController>();
        spare.isScepter = primary.isScepter;

        return spare;
    }

    private static bool IsPrimary(ChirrFriendController slot) => slot.GetComponent<ChirrFriendController>() == slot;

    private static int FriendLimit(CharacterMaster chirr) => chirr.inventory.GetItemCountEffective(DLC1Content.Items.EquipmentMagazineVoid) > 0
        ? 2
        : 1;
}
