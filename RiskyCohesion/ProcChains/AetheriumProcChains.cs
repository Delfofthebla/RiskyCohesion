using System;
using Aetherium.EliteEquipment;
using Aetherium.Items;
using Aetherium.Items.Tier1;
using Aetherium.Items.Tier3;
using Aetherium.Items.TierLunar;
using BepInEx.Configuration;
using RiskyCohesion.Elites;
using RoR2;
using RoR2.Projectile;

namespace RiskyCohesion.ProcChains;

internal static class AetheriumProcChains
{
    public static ProcChain NailBombNails { get; private set; }
    public static ProcChain InspiringDroneExplosion { get; private set; }

    public static void Install(ConfigFile config)
    {
        var procChains = new Lazy<ProcChainGroup>(() => ProcChainGroup.Bind(config, ContentMod.Aetherium));

        if (IsEnabled(ItemBase<NailBomb>.instance))
        {
            NailBombNails = procChains.Value.Bind("Nail Bomb", "Nail Bomb's nails", 0.5f)
                .PatchFirstStore<BulletAttack>(typeof(NailBomb.NailBombDetonator), nameof(NailBomb.NailBombDetonator.Detonate));
        }

        if (IsEnabled(ItemBase<InspiringDrone>.instance))
        {
            InspiringDroneExplosion = procChains.Value.Bind("Inspiring Drone", "an Inspired drone's death explosion", 1f)
                .PatchFirstStore<BlastAttack>(typeof(InspiringDrone), nameof(InspiringDrone.DetonateInspiredDrone));
        }

        if (IsEnabled(ItemBase<BlasterSword>.instance))
            OverrideBlasterSword(procChains.Value);

        if (IsEnabled(ItemBase<WitchesRing>.instance))
            ProcTrigger.ForItem(config, ContentMod.Aetherium, "Witches Ring").SkipOnBlockedHits<WitchesRing>(nameof(WitchesRing.GrantOnKillEffectsOnHighDamage));

        if (IsEnabled(ItemBase<ZenithAccelerator>.instance))
            ProcTrigger.ForItem(config, ContentMod.Aetherium, "Zenith Accelerator").SkipOnBlockedHits<ZenithAccelerator>(nameof(ZenithAccelerator.AddAttackSpeedBuff));
    }

    public static void InstallSanguine(ConfigFile config)
    {
        if (!AetheriumElites.SanguineIsEnabled)
            return;

        ProcTrigger.ForElite(config, ContentMod.Aetherium, "Sanguine Bleed", "Sanguine's bleed").SkipOnBlockedHits<AffixSanguine>(nameof(AffixSanguine.BleedOnHit));
    }

    private static bool IsEnabled(ItemBase item) => item?.ItemDef != null;

    private static void OverrideBlasterSword(ProcChainGroup procChains)
    {
        var controller = BlasterSword.SwordProjectile.GetComponent<ProjectileController>();

        procChains.Override("Blaster Sword", "Blaster Sword's sword projectile", controller.procCoefficient, value => controller.procCoefficient = value);
    }
}
