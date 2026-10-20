namespace RiskyCohesion.Elites;

internal sealed record EliteTier(string Name, EliteRework Rework, TierMultiplier Health, TierMultiplier Damage)
{
    public static readonly EliteTier Tier1 = new("Tier 1", EliteRework.Blazing, Health: new TierMultiplier(Vanilla: 4f, Reworked: 3f), Damage: new TierMultiplier(Vanilla: 2f, Reworked: 1.5f));
    public static readonly EliteTier Tier2 = new("Tier 2", EliteRework.Malachite, Health: new TierMultiplier(Vanilla: 18f, Reworked: 16f), Damage: new TierMultiplier(Vanilla: 6f, Reworked: 4f));
}
