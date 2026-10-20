using System;
using RoR2.Orbs;

namespace RiskyCohesion.ProcChains;

internal sealed class OrbProcCoefficient<TOrb>(Func<float> coefficient, Action<TOrb, float> write) where TOrb : Orb
{
    public void Hook(Rule rule) => rule.Hook(typeof(TOrb), nameof(Orb.Begin), Begin);

    private void Begin(Action<TOrb> orig, TOrb orb)
    {
        write(orb, coefficient());
        orig(orb);
    }
}
