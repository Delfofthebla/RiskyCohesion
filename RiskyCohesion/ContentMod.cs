namespace RiskyCohesion;

internal sealed record ContentMod(string Name)
{
    public string ItemsSection { get; } = $"Items - {Name}";
    public string ElitesSection { get; } = $"Elites - {Name}";
    public string SurvivorsSection { get; } = $"Survivors - {Name}";
    public string DronesSection { get; } = $"Drones - {Name}";
    public string WavesSection { get; } = $"Waves - {Name}";
    public string ArtifactsSection { get; } = $"Artifacts - {Name}";

    public static readonly ContentMod Aetherium = new("Aetherium");
    public static readonly ContentMod Sandswept = new("Sandswept");
    public static readonly ContentMod Starstorm = new("Starstorm 2");
    public static readonly ContentMod Spikestrip = new("Spikestrip 2.0");
    public static readonly ContentMod SimulacrumAdditions = new("Simulacrum Additions");
    public static readonly ContentMod ArtificerExtended = new("ArtificerExtended");
    public static readonly ContentMod AncientScepter = new("Ancient Scepter");
    public static readonly ContentMod StormboundFalseSon = new("StormboundFalseSon");
    public static readonly ContentMod ChefOvercooked = new("Chef Overcooked");
    public static readonly ContentMod EggsSkills = new("Eggs Skills");
    public static readonly ContentMod Voidcrid = new("Voidcrid");
}
