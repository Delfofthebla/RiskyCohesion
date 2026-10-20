using BepInEx.Configuration;
using MonoMod.Cil;
using RoR2;
using SS2;

namespace RiskyCohesion.Fixes;

internal static class StarstormEliteEventHealthBonus
{
    public static void Install(ConfigFile config)
    {
        var description = new Description("Starstorm 2's elite events give their health bonus as health instead of damage.");
        var giveAsHealth = new Rule(config.Bind(ContentMod.Starstorm.ElitesSection, "Elite Event Health Bonus", true, description));

        giveAsHealth.HookIL(typeof(EliteEventMissionController), nameof(EliteEventMissionController.OnSpawnedServer), GiveHealthBonusAsHealth);
    }

    private static void GiveHealthBonusAsHealth(ILContext il)
    {
        var cursor = new ILCursor(il);
        var foundHealthBonus = cursor.TryGotoNext(instruction => instruction.MatchLdsfld(typeof(RoR2Content.Items), nameof(RoR2Content.Items.BoostHp)))
            && cursor.TryGotoNext(MoveType.After, instruction => instruction.MatchLdsfld(typeof(RoR2Content.Items), nameof(RoR2Content.Items.BoostDamage)));

        if (!foundHealthBonus)
        {
            RiskyCohesionPlugin.Log.LogError("Could not find the elite event's health bonus; it keeps giving damage instead.");
            return;
        }

        cursor.Prev.Operand = il.Import(typeof(RoR2Content.Items).GetField(nameof(RoR2Content.Items.BoostHp)));
    }
}
