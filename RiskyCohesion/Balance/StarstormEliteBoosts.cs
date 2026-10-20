using System;
using BepInEx.Configuration;
using RoR2;
using SS2;
using SS2.Components;
using UnityEngine;

namespace RiskyCohesion.Balance;

internal static class StarstormEliteBoosts
{
    private static BoostScales _ethereal;
    private static BoostScales _ultra;
    private static BoostScales _empyrean;
    private static BoostScales _eliteEventBoss;

    public static void Install(ConfigFile config)
    {
        _ethereal = BoostScales.Bind(
            config,
            "Ethereal",
            "+3,000% at one ethereal teleporter, +1,500% per further one, +3% or more max health per minute",
            "+400% at one ethereal teleporter, +1,000% at two, +2,000% at three");
        _ethereal.Hook(typeof(Ethereal), nameof(Ethereal.MakeElite), MakeEthereal);
        _ethereal.Hook(typeof(TeleporterUpgradeController), nameof(TeleporterUpgradeController.ModifySpawnedBoss), ModifyEtherealTeleporterBoss);

        _ultra = BoostScales.Bind(config, "Ultra", "+24,000% at one ethereal teleporter, +8,000% per further one, +3% or more max health per minute", "+1,000%");
        _ultra.Hook(typeof(Ultra), nameof(Ultra.MakeElite), MakeUltra);

        _empyrean = BoostScales.Bind(config, "Empyrean", "+40,000% replacing the elite tier's, +3% or more max health per minute", "+1,000% replacing the elite tier's");
        _empyrean.Hook(typeof(Empyrean), nameof(Empyrean.MakeElite), MakeEmpyrean);

        _eliteEventBoss = BoostScales.BindHealthGrowth(config, "Elite Event Boss", "+2% max health per minute before looping, +8% after one loop, about +46% after two");
        _eliteEventBoss.Hook(typeof(EliteEventMissionController), nameof(EliteEventMissionController.OnBossSpawned), OnEliteEventBossSpawned);
    }

    private static void MakeEthereal(Action<Ethereal, CharacterBody> orig, Ethereal self, CharacterBody body)
        => ScaleBoosts(body.inventory, _ethereal, () => orig(self, body), before => before.ScaleAdded());

    private static void MakeUltra(Action<Ultra, CharacterBody> orig, Ultra self, CharacterBody body) => ScaleBoosts(body.inventory, _ultra, () => orig(self, body), before => before.ScaleAdded());

    private static void MakeEmpyrean(Action<Empyrean, CharacterBody> orig, Empyrean self, CharacterBody body)
        => ScaleBoosts(body.inventory, _empyrean, () => orig(self, body), before => before.ScaleReplaced());

    private static void ModifyEtherealTeleporterBoss(Action<TeleporterUpgradeController, GameObject> orig, TeleporterUpgradeController self, GameObject masterObject)
        => ScaleBoosts(masterObject.GetComponent<Inventory>(), _ethereal, () => orig(self, masterObject), before => before.ScaleAdded());

    private static void OnEliteEventBossSpawned(Action<EliteEventMissionController, GameObject> orig, EliteEventMissionController self, GameObject masterObject)
        => ScaleBoosts(masterObject.GetComponent<Inventory>(), _eliteEventBoss, () => orig(self, masterObject), before => before.ScaleAddedGrowth());

    private static void ScaleBoosts(Inventory inventory, BoostScales scales, Action makeElite, Action<BoostCounts> scale)
    {
        if (!inventory)
        {
            makeElite();
            return;
        }

        var before = new BoostCounts(inventory, scales);
        makeElite();
        scale(before);
    }

    private readonly struct BoostCounts(Inventory inventory, BoostScales scales)
    {
        private readonly int _health = inventory.GetItemCountPermanent(RoR2Content.Items.BoostHp);
        private readonly int _damage = inventory.GetItemCountPermanent(RoR2Content.Items.BoostDamage);
        private readonly int _growth = inventory.GetItemCountPermanent(SS2Content.Items.MaxHealthPerMinute);

        public void ScaleAdded()
        {
            ScaleAddedItems(RoR2Content.Items.BoostHp, _health, scales.HealthScale);
            ScaleAddedItems(RoR2Content.Items.BoostDamage, _damage, scales.DamageScale);
            ScaleAddedGrowth();
        }

        public void ScaleReplaced()
        {
            ScaleAllItems(RoR2Content.Items.BoostHp, scales.HealthScale);
            ScaleAllItems(RoR2Content.Items.BoostDamage, scales.DamageScale);
            ScaleAddedGrowth();
        }

        public void ScaleAddedGrowth() => ScaleAddedItems(SS2Content.Items.MaxHealthPerMinute, _growth, scales.HealthScale);

        private void ScaleAddedItems(ItemDef item, int before, float scale)
        {
            var added = inventory.GetItemCountPermanent(item) - before;
            if (added > 0)
                SetCount(item, before + Scaled(added, scale));
        }

        private void ScaleAllItems(ItemDef item, float scale) => SetCount(item, Scaled(inventory.GetItemCountPermanent(item), scale));

        private void SetCount(ItemDef item, int count)
        {
            var difference = count - inventory.GetItemCountPermanent(item);
            if (difference > 0)
                inventory.GiveItemPermanent(item, difference);
            else if (difference < 0)
                inventory.RemoveItemPermanent(item, -difference);
        }

        private static int Scaled(int count, float scale) => (int)Math.Round(count * scale, MidpointRounding.AwayFromZero);
    }
}
