namespace RiskyCohesion;

internal sealed record ContentMod(string Name)
{
    public string ItemsSection { get; } = $"Items - {Name}";
    public string ElitesSection { get; } = $"Elites - {Name}";
    public string SurvivorsSection { get; } = $"Survivors - {Name}";
    public string DronesSection { get; } = $"Drones - {Name}";

    public static readonly ContentMod Aetherium = new("Aetherium");
    public static readonly ContentMod Sandswept = new("Sandswept");
    public static readonly ContentMod Starstorm = new("Starstorm 2");
    public static readonly ContentMod Spikestrip = new("Spikestrip 2.0");
}
