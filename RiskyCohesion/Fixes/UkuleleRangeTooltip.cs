using BepInEx.Configuration;
using RiskyCohesion.Sources;

namespace RiskyCohesion.Fixes;

internal static class UkuleleRangeTooltip
{
    private static readonly string DescriptionToken = "ITEM_CHAINLIGHTNING_DESC";

    public static void Install(ConfigFile config)
    {
        var description = new Description("Corrects the range in Ukulele's description.");
        var correctTooltip = new Rule(config.BindWhen(!RiskyModSource.ChangesUkulele, Sections.Vanilla, "Ukulele Range Tooltip", true, description));

        new TooltipCorrection(DescriptionToken, ItemValues.UkuleleRange, ItemValues.UkuleleRange + ItemValues.UkuleleRangePerStack).Hook(correctTooltip);
    }
}
