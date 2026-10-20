using System;
using BepInEx.Configuration;
using RiskyCohesion.Sources;

namespace RiskyCohesion.ProcChains;

internal sealed class ProcChainGroup(ConfigFile config, ContentMod contentMod, ConfigEntry<bool> enabled) : Rule(enabled)
{
    private static readonly string ToggleKey = "Proc Chains";
    private static readonly string VanillaItemDamage = "vanilla item damage";
    private static readonly float RiskyModCoefficient = 0f;

    public static ProcChainGroup Bind(ConfigFile config, ContentMod contentMod)
    {
        var description = new Description("Applies this section's proc coefficients.")
            .Requires(RiskyModSource.DisableProcChainsSetting);

        return new ProcChainGroup(config, contentMod, config.BindWhen(RiskyModSource.DisablesProcChains, contentMod.ItemsSection, ToggleKey, true, description));
    }

    public ProcChain Bind(string itemName, string damage, float contentModValue)
    {
        var description = new Description($"Proc coefficient of {damage}.")
            .Requires(ToggleKey)
            .Reference(contentMod.Name, Description.Format(contentModValue))
            .Reference("RiskyMod", $"{Description.Format(RiskyModCoefficient)} ({VanillaItemDamage})");

        return new ProcChain(this, config.BindWhen(SourceIsOn, contentMod.ItemsSection, itemName, RiskyModCoefficient, description), itemName, contentModValue);
    }

    public void Override(string itemName, string damage, float original, Action<float> write) => Bind(itemName, damage, original).WriteTo(original, write);
}
