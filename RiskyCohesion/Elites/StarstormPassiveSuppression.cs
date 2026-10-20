using System;
using BepInEx.Configuration;
using SS2.Equipments;
using SS2.Items;

namespace RiskyCohesion.Elites;

internal static class StarstormPassiveSuppression
{
    private static ElitePassiveSuppression _ultra;

    public static void Install(ConfigFile config)
    {
        var ethereal = ElitePassiveSuppression.Bind(config, ContentMod.Starstorm, "Ethereal", "Ethereal's circles");
        new StunResetTimer<AffixEthereal.EtherealBehavior>(ethereal, behaviour => behaviour.characterBody, behaviour => behaviour.timer = 0f).Install();

        var tectonic = ElitePassiveSuppression.Bind(config, ContentMod.Starstorm, "Tectonic", "Tectonic's sunders");
        new StunResetTimer<AffixSuperEarth.Behavior>(tectonic, behaviour => behaviour.characterBody, behaviour => behaviour.projectileTimer = 0f).Install();

        var infernal = ElitePassiveSuppression.Bind(config, ContentMod.Starstorm, "Infernal", "Infernal's fireballs");
        new StunResetTimer<AffixSuperFire.Behavior>(infernal, behaviour => behaviour.characterBody, behaviour => behaviour.projectileTimer = 0f).Install();

        var boreal = ElitePassiveSuppression.Bind(config, ContentMod.Starstorm, "Boreal", "Boreal's meteors");
        new StunResetTimer<AffixSuperIce.Behavior>(boreal, behaviour => behaviour.characterBody, behaviour => behaviour.projectileTimer = 0f).Install();

        var supercharged = ElitePassiveSuppression.Bind(config, ContentMod.Starstorm, "Supercharged", "Supercharged's ally buff orbs");
        new StunResetTimer<AffixSuperLightning.Behavior>(supercharged, behaviour => behaviour.characterBody, behaviour => behaviour.orbTimer = 0f).Install();

        _ultra = ElitePassiveSuppression.Bind(config, ContentMod.Starstorm, "Ultra", "Ultra's ward");
        _ultra.Hook(typeof(AffixUltra.BodyBehavior), nameof(AffixUltra.BodyBehavior.Start), AttachUltraWard);
    }

    private static void AttachUltraWard(Action<AffixUltra.BodyBehavior> orig, AffixUltra.BodyBehavior self)
    {
        orig(self);
        StunCollapsedWard.Attach(self.wardInstance, self.body, _ultra);
    }
}
