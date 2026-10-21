using System.Linq;
using BepInEx.Configuration;
using EntityStates.VoidInfestor;
using RiskyCohesion.Sources;
using RoR2;
using RoR2.Achievements;
using SS2.Components;
using SS2.Unlocks.Chirr;

namespace RiskyCohesion.Fixes;

internal static class StarstormInfestedAllies
{
    public static void Install(ConfigFile config)
    {
        var description = new Description("An enemy infested by Chirr's befriended Void Infestor becomes Chirr's friend in its place, and grants Chirr: Betrayed.")
            .Requires(RiskyModSource.NoAllyInfestationSetting);

        var settleInfests = new Rule(config.BindWhen(RiskyModSource.PreventsAllyInfestation, ContentMod.Starstorm.SurvivorsSection, "Infested Allies", true, description));

        settleInfests.Subscribe(() => GlobalEventManager.onCharacterDeathServer += SettleInfest, () => GlobalEventManager.onCharacterDeathServer -= SettleInfest);
    }

    private static void SettleInfest(DamageReport death)
    {
        var chirr = ChirrFriends.BefriendingChirr(death.victimMaster);
        if (!chirr)
            return;

        var host = InfestedHost(death.victimBody);
        if (!host)
            return;

        Failsafe.Run("passing a Void Infestor's friendship to its host", () => PassFriendship(chirr, death.victimMaster, host));
        GrantBetrayed(chirr);
    }

    private static CharacterMaster InfestedHost(CharacterBody infestor)
    {
        var infest = infestor.GetComponents<EntityStateMachine>().Select(machine => machine.state).OfType<Infest>().FirstOrDefault();
        if (infest == null)
            return null;

        return infest.victimsStruck
            .Where(hurtBox => hurtBox && hurtBox.healthComponent.body.teamComponent.teamIndex == TeamIndex.Player)
            .Select(hurtBox => hurtBox.healthComponent.body.master)
            .FirstOrDefault();
    }

    private static void PassFriendship(ChirrFriendController chirr, CharacterMaster infestor, CharacterMaster host)
    {
        chirr.RemoveFriend(infestor);

        // AddFriend reads the friend's current team to decide where releasing it returns it and whether to strip its Void affix; vanilla's infest leaves the host on the Void team.
        host.teamIndex = TeamIndex.Void;
        host.GetBody().teamComponent.teamIndex = TeamIndex.Void;

        chirr.AddFriend(host);
    }

    private static void GrantBetrayed(ChirrFriendController chirr)
    {
        if (!chirr.TryGetComponent<PlayerCharacterMasterController>(out var player) || !player.networkUser)
            return;

        var betrayed = player.networkUser.GetComponent<ServerAchievementTracker>().achievementTrackers.OfType<ChirrIsopodAchievement.ChirrIsopodServerAchievement>().FirstOrDefault();

        betrayed?.Grant();
    }
}
