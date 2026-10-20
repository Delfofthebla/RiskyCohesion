using System;
using RoR2;

namespace RiskyCohesion.Elites;

internal sealed class StunResetTimer<TBehaviour>(ElitePassiveSuppression suppression, Func<TBehaviour, CharacterBody> owner, Action<TBehaviour> resetTimer)
{
    private static readonly string FixedUpdateMethod = "FixedUpdate";

    public void Install() => suppression.Hook(typeof(TBehaviour), FixedUpdateMethod, FixedUpdate);

    private void FixedUpdate(Action<TBehaviour> orig, TBehaviour self)
    {
        if (ElitePassiveSuppression.Suppresses(owner(self)))
            resetTimer(self);

        orig(self);
    }
}
