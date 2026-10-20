using System;
using BepInEx.Configuration;
using RoR2;
using Sandswept.Elites;
using OrigTakeDamage = On.RoR2.HealthComponent.orig_TakeDamage;

namespace RiskyCohesion.Elites;

internal static class SandsweptElites
{
    private static ElitePassiveSuppression _motivating;
    private static ElitePassiveSuppression _osmium;

    public static void Install(ConfigFile config)
    {
        if (EliteEquipmentBase<Motivating>.Instance?.EliteDef != null)
            InstallMotivating(config);

        if (EliteEquipmentBase<Osmium>.Instance?.EliteDef != null)
            InstallOsmium(config);
    }

    private static void InstallMotivating(ConfigFile config)
    {
        _motivating = ElitePassiveSuppression.Bind(config, ContentMod.Sandswept, "Motivating", "Motivating's speed aura");
        _motivating.Hook(typeof(MotivatorController), nameof(MotivatorController.Start), AttachMotivatingWard);
    }

    private static void InstallOsmium(ConfigFile config)
    {
        _osmium = ElitePassiveSuppression.Bind(config, ContentMod.Sandswept, "Osmium", "Osmium's aura");
        _osmium.Hook(typeof(OsmiumController), nameof(OsmiumController.Start), AttachOsmiumWard);
        _osmium.Hook(typeof(Osmium), nameof(Osmium.HealthComponent_TakeDamage), SkipAuraForStunnedOsmium);
        new StunResetTimer<OsmiumController>(_osmium, controller => controller.body, controller => controller.timer = 0f).Install();
    }

    private static void AttachMotivatingWard(Action<MotivatorController> orig, MotivatorController self)
    {
        orig(self);
        StunCollapsedWard.Attach(self.warbannerInstance, self.body, _motivating);
    }

    private static void AttachOsmiumWard(Action<OsmiumController> orig, OsmiumController self)
    {
        orig(self);
        StunCollapsedWard.Attach(self.wardInstance, self.body, _osmium);
    }

    private static void SkipAuraForStunnedOsmium(Action<Osmium, OrigTakeDamage, HealthComponent, DamageInfo> orig, Osmium self, OrigTakeDamage next, HealthComponent victim, DamageInfo damageInfo)
    {
        if (IsStunnedOsmium(victim))
            next(victim, damageInfo);
        else
            orig(self, next, victim, damageInfo);
    }

    private static bool IsStunnedOsmium(HealthComponent victim)
    {
        var body = victim ? victim.body : null;

        return body && body.HasBuff(EliteEquipmentBase<Osmium>.Instance.EliteBuffDef) && ElitePassiveSuppression.Suppresses(body);
    }
}
