using System.Runtime.CompilerServices;
using EliteReworks2;
using EliteReworks2.Elites.Blazing;
using EliteReworks2.Elites.Malachite;
using EliteReworks2.Elites.Overloading;
using RoR2;

namespace RiskyCohesion.Sources;

internal static class EliteReworksSource
{
    public static bool IsInstalled { get; private set; }
    public static bool ReworksBlazing { get; private set; }
    public static bool ReworksOverloadingLightning { get; private set; }
    public static bool ReworksMalachite { get; private set; }
    public static FlatDamage MalachiteSpikes { get; private set; }
    public static BuffDef PassiveSuppressionBuff { get; private set; }

    // EliteReworks keeps these as field defaults of PassiveLightningComponent, which only exists on a live Overloading elite.
    public static FlatDamage OverloadingBombs { get; } = new(36f, 57.6f);

    public static void Load()
    {
        IsInstalled = Mods.IsLoaded(Mods.ELITE_REWORKS);
        if (IsInstalled)
            ReadSettings();
    }

    public static float AmbientScaled(float damage) => damage * AmbientLevelDamageScalar();

    // EliteReworks' types only resolve when EliteReworks is loaded, so they stay out of any method that runs without it.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ReadSettings()
    {
        ReworksBlazing = Blazing.Instance?.Enabled.Value == true;
        ReworksOverloadingLightning = Overloading.Instance?.Enabled.Value == true && Overloading.passiveLightning;
        ReworksMalachite = Malachite.Instance?.Enabled.Value == true;

        MalachiteSpikes = new FlatDamage(Malachite.spikeDamage, Malachite.spikeDamageBoss);
        PassiveSuppressionBuff = EliteReworks2.Common.Buffs.DisablePassiveEffect;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static float AmbientLevelDamageScalar() => EliteReworks2Utils.GetAmbientLevelDamageScalar();
}
