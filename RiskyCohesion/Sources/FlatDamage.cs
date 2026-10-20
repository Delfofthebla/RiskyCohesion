using RoR2;

namespace RiskyCohesion.Sources;

internal sealed class FlatDamage(float normal, float champion)
{
    public float Replace(float original, CharacterBody holder)
    {
        if (!holder || holder.isPlayerControlled || holder.teamComponent.teamIndex == TeamIndex.Player)
            return original;

        var damage = holder.isChampion || holder.isBoss
            ? champion
            : normal;

        return EliteReworksSource.AmbientScaled(damage);
    }
}
