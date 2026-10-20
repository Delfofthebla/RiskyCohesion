using System;
using BepInEx.Configuration;

namespace RiskyCohesion.Balance;

internal sealed class BoostScales(ConfigEntry<float> healthScale, ConfigEntry<float> damageScale) : ISwitch
{
    private static readonly float DefaultScale = 1f;
    private static readonly AcceptableValueRange<float> ScaleRange = new(0f, 10f);
    private static readonly string VanillaHealthBonus = "Tier 1 elites +300%, Tier 2 +1,700%";
    private static readonly string VanillaDamageBonus = "Tier 1 elites +100%, Tier 2 +500%";

    public float HealthScale => healthScale.Value;
    public float DamageScale => damageScale?.Value ?? DefaultScale;
    public bool IsOn => HealthScale != DefaultScale || DamageScale != DefaultScale;

    public static BoostScales Bind(ConfigFile config, string affixName, string starstormHealth, string starstormDamage)
    {
        var healthScale = BindScale(
            config,
            $"{affixName} Health Scale",
            ScaleDescription($"Multiplies Starstorm 2's health bonus for {affixName}, max health growth included.", starstormHealth).Vanilla(VanillaHealthBonus));

        var damageScale = BindScale(
            config,
            $"{affixName} Damage Scale",
            ScaleDescription($"Multiplies Starstorm 2's damage bonus for {affixName}.", starstormDamage).Vanilla(VanillaDamageBonus));

        return new BoostScales(healthScale, damageScale);
    }

    public static BoostScales BindHealthGrowth(ConfigFile config, string name, string starstormGrowth)
    {
        var healthScale = BindScale(config, $"{name} Health Scale", ScaleDescription($"Multiplies Starstorm 2's max health growth for the {name}.", starstormGrowth));

        return new BoostScales(healthScale, null);
    }

    public void Hook(Type type, string methodName, Delegate hook) => Hooks.Install(type, methodName, hook, this);

    public void ApplyNowAndOnChange(Action apply)
    {
        healthScale.ApplyNowAndOnChange(apply);

        if (damageScale != null)
            damageScale.SettingChanged += (_, _) => apply();
    }

    private static Description ScaleDescription(string summary, string starstorm) => new Description(summary)
        .Reference(ContentMod.Starstorm.Name, starstorm);

    private static ConfigEntry<float> BindScale(ConfigFile config, string key, Description description)
        => config.Bind(Sections.Balance, key, DefaultScale, new ConfigDescription(description, ScaleRange));
}
