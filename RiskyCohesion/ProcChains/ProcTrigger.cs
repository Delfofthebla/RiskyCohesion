using System;
using BepInEx.Configuration;
using RiskyCohesion.Sources;
using RoR2;
using UnityEngine;
using OrigOnHitEnemy = On.RoR2.GlobalEventManager.orig_OnHitEnemy;

namespace RiskyCohesion.ProcChains;

internal sealed class ProcTrigger(ConfigEntry<bool> enabled) : Rule(enabled)
{
    public static ProcTrigger ForItem(ConfigFile config, ContentMod contentMod, string itemName) => Bind(config, contentMod.ItemsSection, itemName, itemName);

    public static ProcTrigger ForElite(ConfigFile config, ContentMod contentMod, string key, string effect) => Bind(config, contentMod.ElitesSection, key, effect);

    public static bool Blocks(DamageInfo damageInfo) => damageInfo.procCoefficient <= 0f;

    public void SkipOnBlockedHits<TOwner>(string onHitEnemyHandler) => Hook(typeof(TOwner), onHitEnemyHandler, new BlockedHitSkip<TOwner>().OnHitEnemy);

    private static ProcTrigger Bind(ConfigFile config, string section, string key, string trigger)
    {
        var description = new Description($"{trigger} only triggers from hits with proc coefficient above 0.")
            .Requires(RiskyModSource.DisableProcChainsSetting);

        return new ProcTrigger(config.BindWhen(RiskyModSource.DisablesProcChains, section, key, true, description));
    }

    private sealed class BlockedHitSkip<TOwner>
    {
        public void OnHitEnemy(
            Action<TOwner, OrigOnHitEnemy, GlobalEventManager, DamageInfo, GameObject> orig,
            TOwner self,
            OrigOnHitEnemy next,
            GlobalEventManager manager,
            DamageInfo damageInfo,
            GameObject victim)
        {
            if (Blocks(damageInfo))
                next(manager, damageInfo, victim);
            else
                orig(self, next, manager, damageInfo, victim);
        }
    }
}
