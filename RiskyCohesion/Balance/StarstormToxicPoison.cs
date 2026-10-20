using System;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using SS2.Equipments;
using UnityEngine;
using OrigOnHitAllProcess = On.RoR2.GlobalEventManager.orig_OnHitAllProcess;

namespace RiskyCohesion.Balance;

internal static class StarstormToxicPoison
{
    public static void Install(ConfigFile config)
    {
        var description = new Description("Starstorm 2's Toxic Elite's Poison rebalance. When enabled, each poison stack expires on its own, and blocked hits don't splash poison."
            + " Prevents insane overstacking and instakills.");

        var poison = new Rule(config.Bind(Sections.Balance, "Toxic Poison", false, description));

        poison.HookIL(typeof(AffixPurple), nameof(AffixPurple.OnServerDamageDealt), StopRefreshingPoison);
        poison.Hook(typeof(AffixPurple), nameof(AffixPurple.OnHitAllProcess), SkipBlockedSplashes);
    }

    private static void StopRefreshingPoison(ILContext il)
    {
        var cursor = new ILCursor(il);
        if (!cursor.TryGotoNext(MoveType.After, instruction => instruction.MatchCall(typeof(DotController), nameof(DotController.InflictDot))))
        {
            RiskyCohesionPlugin.Log.LogError("Could not find where Toxic applies its poison; its stacks keep refreshing each other.");
            return;
        }

        cursor.Emit(OpCodes.Ret);
    }

    private static void SkipBlockedSplashes(
        Action<AffixPurple, OrigOnHitAllProcess, GlobalEventManager, DamageInfo, GameObject> orig,
        AffixPurple self,
        OrigOnHitAllProcess next,
        GlobalEventManager manager,
        DamageInfo damageInfo,
        GameObject hitObject)
    {
        if (damageInfo.rejected)
            next(manager, damageInfo, hitObject);
        else
            orig(self, next, manager, damageInfo, hitObject);
    }
}
