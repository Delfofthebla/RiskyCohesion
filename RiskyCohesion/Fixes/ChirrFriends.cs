using System.Linq;
using RoR2;
using SS2.Components;

namespace RiskyCohesion.Fixes;

internal static class ChirrFriends
{
    public static ChirrFriendController BefriendingChirr(CharacterMaster friend)
    {
        var owner = friend ? friend.minionOwnership.ownerMaster : null;
        if (!owner)
            return null;

        return owner.GetComponents<ChirrFriendController>().FirstOrDefault(chirr => chirr.currentFriend.master == friend);
    }
}
