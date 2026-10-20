using System;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2.Orbs;

namespace RiskyCohesion.ProcChains;

internal sealed class ProcChain(ProcChainGroup group, ConfigEntry<float> coefficient, string itemName, float contentModValue)
{
    private static readonly string ProcCoefficientField = "procCoefficient";

    public ProcChain PatchFirstStore<TAttack>(Type type, string methodName)
    {
        group.HookIL(type, methodName, ReplaceFirstStore<TAttack>);
        return this;
    }

    public ProcChain SetOnBegin<TOrb>(Action<TOrb, float> write) where TOrb : Orb
    {
        new OrbProcCoefficient<TOrb>(Coefficient, write).Hook(group);
        return this;
    }

    public float CoefficientInEffect() => group.SourceIsOn && group.IsOn
        ? coefficient.Value
        : contentModValue;

    public void WriteTo(float original, Action<float> write)
    {
        if (!group.SourceIsOn)
            return;

        group.ApplyNowAndOnChange(() => Write(original, write));
        coefficient.SettingChanged += (_, _) => Write(original, write);
    }

    private void Write(float original, Action<float> write) => write(group.IsOn ? coefficient.Value : original);

    private void ReplaceFirstStore<TAttack>(ILContext il)
    {
        var cursor = new ILCursor(il);
        if (!cursor.TryGotoNext(instruction => instruction.MatchStfld<TAttack>(ProcCoefficientField)))
        {
            RiskyCohesionPlugin.Log.LogError($"Could not find {itemName}'s proc coefficient in {il.Method.Name}; it keeps its own value.");
            return;
        }

        cursor.Emit(OpCodes.Pop);
        cursor.EmitDelegate(Coefficient);
    }

    private float Coefficient() => coefficient.Value;
}
