using System;
using BepInEx.Configuration;
using MonoMod.Cil;

namespace RiskyCohesion;

internal class Rule(ConfigEntry<bool> enabled) : ISwitch
{
    public ConfigEntry<bool> Enabled { get; } = enabled;
    public bool SourceIsOn => Enabled != null;
    public bool IsOn => Enabled.Value;

    public void Hook(Type type, string methodName, Delegate hook)
    {
        if (SourceIsOn)
            Hooks.Install(type, methodName, hook, this);
    }

    public void HookIL(Type type, string methodName, ILContext.Manipulator manipulator)
    {
        if (SourceIsOn)
            Hooks.InstallIL(type, methodName, manipulator, this);
    }

    public void Subscribe(Action subscribe, Action unsubscribe)
    {
        if (SourceIsOn)
            ApplyNowAndOnChange(() => FollowSetting(subscribe, unsubscribe));
    }

    public void Override(Func<float> read, Action<float> write, Func<float, float> rework)
    {
        if (!SourceIsOn)
            return;

        var original = read();

        ApplyNowAndOnChange(() => write(ValueInEffect(original, rework)));
    }

    public void ApplyNowAndOnChange(Action apply) => Enabled.ApplyNowAndOnChange(apply);

    private void FollowSetting(Action subscribe, Action unsubscribe)
    {
        unsubscribe();

        if (IsOn)
            subscribe();
    }

    private float ValueInEffect(float original, Func<float, float> rework) => IsOn
        ? rework(original)
        : original;
}
