using System;
using System.Collections.Generic;

namespace RiskyCohesion;

internal sealed class SharedHook<THandler>(Type type, string methodName, Delegate hook) : ISwitch
{
    private readonly List<THandler> _handlers = [];

    private Action _activeChanged;
    private bool _installed;

    public THandler[] Active { get; private set; } = [];
    public bool IsOn => Active.Length > 0;

    public void Add(THandler handler, Rule rule)
    {
        if (!rule.SourceIsOn)
            return;

        InstallOnce();
        rule.Subscribe(() => Activate(handler), () => Deactivate(handler));
    }

    public void ApplyNowAndOnChange(Action apply)
    {
        apply();
        _activeChanged += apply;
    }

    private void InstallOnce()
    {
        if (_installed)
            return;

        _installed = true;
        Hooks.Install(type, methodName, hook, this);
    }

    private void Activate(THandler handler)
    {
        _handlers.Add(handler);
        Publish();
    }

    private void Deactivate(THandler handler)
    {
        if (_handlers.Remove(handler))
            Publish();
    }

    private void Publish()
    {
        Active = [.. _handlers];
        _activeChanged?.Invoke();
    }
}
