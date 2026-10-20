using BepInEx.Configuration;
using RoR2;
using RoR2.Orbs;
using SS2.Components;
using SS2.Items;

namespace RiskyCohesion.ProcChains;

internal static class StarstormProcChains
{
    public static ProcChain BlastKnucklesBlast { get; private set; }
    public static ProcChain DebuffMissilesMissiles { get; private set; }
    public static ProcChain ErraticGadgetLightning { get; private set; }
    public static ProcChain NeedlesStrikes { get; private set; }
    public static ProcChain StickyOverloaderDetonation { get; private set; }

    public static void Install(ConfigFile config)
    {
        var procChains = ProcChainGroup.Bind(config, ContentMod.Starstorm);

        BlastKnucklesBlast = procChains.Bind("Blast Knuckles", "Blast Knuckles' blast", 1f)
            .PatchFirstStore<BulletAttack>(typeof(BlastKnuckles), nameof(BlastKnuckles.OnHitEnemy));

        DebuffMissilesMissiles = procChains.Bind("DebuffMissiles", "DebuffMissiles' missiles", 1f)
            .PatchFirstStore<GenericDamageOrb>(typeof(DebuffMissiles.Behavior), nameof(DebuffMissiles.Behavior.OnDamageDealtServer));

        ErraticGadgetLightning = procChains.Bind("Erratic Gadget", "Erratic Gadget's on-hit lightning", 1f)
            .PatchFirstStore<ErraticGadget.GadgetLightningOrb>(typeof(ErraticGadget.Behavior), nameof(ErraticGadget.Behavior.OnDamageDealtServer));

        NeedlesStrikes = procChains.Bind("Needles", "Needles' strikes", Needles.needleProcCoefficient)
            .SetOnBegin<Needles.NeedleOrb>((orb, value) => orb.procCoefficient = value);

        StickyOverloaderDetonation = procChains.Bind("Sticky Overloader", "Sticky Overloader's detonation", 1f)
            .PatchFirstStore<BlastAttack>(typeof(StickyOverloaderController), nameof(StickyOverloaderController.FireBlast));
    }
}
