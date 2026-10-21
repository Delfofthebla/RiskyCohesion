using System;
using BepInEx.Configuration;
using EnemyAbilities.Modules.Beetle;
using RoR2;
using SS2.Artifacts;

namespace RiskyCohesion.Fixes;

internal static class StarstormCognationBeetleSummons
{
    public static void Install(ConfigFile config)
    {
        var description = new Description("Cognation ghosts keep the EnemyAbilities summon depth of the beetle they copy.");
        var keepSummonDepth = new Rule(config.Bind(ContentMod.Starstorm.ArtifactsSection, "Cognation Beetle Summons", true, description));

        keepSummonDepth.Hook(typeof(Cognation), nameof(Cognation.SummonGhost), KeepSummonDepth);
    }

    private static void KeepSummonDepth(Action<Cognation, CharacterMaster, CharacterMaster> orig, Cognation self, CharacterMaster ghost, CharacterMaster original)
    {
        orig(self, ghost, original);

        var ghostSummons = SummonController(ghost);
        var originalSummons = SummonController(original);
        if (ghostSummons && originalSummons)
            ghostSummons.recursionDepth = originalSummons.recursionDepth;
    }

    private static BeetleSwarmSkill.BeetleSummonController SummonController(CharacterMaster master) => master.GetBodyObject().ComponentOrNull<BeetleSwarmSkill.BeetleSummonController>();
}
