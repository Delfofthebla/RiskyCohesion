using System;
using RoR2;

namespace RiskyCohesion.Elites;

internal sealed class StunSkippedDamageReaction<TBehaviour>(ElitePassiveSuppression suppression, Func<TBehaviour, CharacterBody> owner) where TBehaviour : IOnTakeDamageServerReceiver
{
    public void Install() => suppression.Hook(typeof(TBehaviour), nameof(IOnTakeDamageServerReceiver.OnTakeDamageServer), OnTakeDamageServer);

    private void OnTakeDamageServer(Action<TBehaviour, DamageReport> orig, TBehaviour self, DamageReport damageReport)
    {
        if (!ElitePassiveSuppression.Suppresses(owner(self)))
            orig(self, damageReport);
    }
}
