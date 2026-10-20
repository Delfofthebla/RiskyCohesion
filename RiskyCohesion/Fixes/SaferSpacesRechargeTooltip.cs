using BepInEx.Configuration;
using RiskyCohesion.Sources;

namespace RiskyCohesion.Fixes;

internal static class SaferSpacesRechargeTooltip
{
    private static readonly string VanillaDescriptionToken = "ITEM_BEARVOID_DESC";
    private static readonly string RiskyModDescriptionToken = "ITEM_BEARVOID_DESC_RISKYMOD";

    public static void Install(ConfigFile config)
    {
        var description = new Description("Corrects the recharge time in Safer Spaces' description.")
            .Requires($"{RiskyFixesSource.SaferSpacesCooldownSetting} off");

        var correctTooltip = new Rule(config.BindWhen(!RiskyFixesSource.CorrectsSaferSpacesCooldown, Sections.Vanilla, "Safer Spaces Recharge Tooltip", true, description));

        var (token, statedRecharge) = RiskyModSource.ChangesSaferSpaces
            ? (RiskyModDescriptionToken, ItemValues.SaferSpacesRiskyModRecharge)
            : (VanillaDescriptionToken, ItemValues.SaferSpacesRecharge);

        new TooltipCorrection(token, statedRecharge, statedRecharge * ItemValues.SaferSpacesRechargeKeptPerStack).Hook(correctTooltip);
    }
}
