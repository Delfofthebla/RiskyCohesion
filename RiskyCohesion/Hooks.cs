using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using RoR2;

namespace RiskyCohesion;

// Hooks are created once the game has loaded its content: applying a hook compiles the target method, which can run its type's
// static initializer, and content mods' initializers load assets that only exist once loading is done (Starstorm 2's EtherealBehavior).
internal static class Hooks
{
    private static readonly BindingFlags AnyMethod = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

    private static readonly List<SwitchedHook> _pending = [];

    private static bool _contentLoaded;

    public static void CreateQueuedOnLoad() => RoR2Application.onLoad += CreatePending;

    public static void Install(Type type, string methodName, Delegate hook, ISwitch whileOn)
        => Queue(new SwitchedHook(type, methodName, method => new Hook(method, hook, new HookConfig { ManualApply = true }), whileOn));

    public static void InstallIL(Type type, string methodName, ILContext.Manipulator manipulator, ISwitch whileOn)
        => Queue(new SwitchedHook(type, methodName, method => new ILHook(method, manipulator, new ILHookConfig { ManualApply = true }), whileOn));

    private static void Queue(SwitchedHook hook)
    {
        if (_contentLoaded)
        {
            hook.Create();
            return;
        }

        _pending.Add(hook);
    }

    private static void CreatePending()
    {
        _contentLoaded = true;

        foreach (var hook in _pending)
            hook.Create();

        _pending.Clear();
    }

    private static MethodInfo Find(Type type, string methodName)
    {
        var candidates = type.GetMethods(AnyMethod).Where(method => method.Name == methodName).ToArray();
        if (candidates.Length == 1)
            return candidates[0];

        var failure = candidates.Length == 0
            ? $"Hook target not found: {type.FullName}.{methodName}."
            : $"Hook target is ambiguous: {type.FullName}.{methodName} has {candidates.Length} overloads.";

        RiskyCohesionPlugin.Log.LogError(failure);

        return null;
    }

    private sealed class SwitchedHook(Type type, string methodName, Func<MethodInfo, IDetour> createDetour, ISwitch whileOn)
    {
        private IDetour _detour;

        public void Create()
        {
            var method = Find(type, methodName);
            if (method == null)
                return;

            try
            {
                _detour = createDetour(method);
            }
            catch (Exception exception)
            {
                LogFailure(exception);
                return;
            }

            whileOn.ApplyNowAndOnChange(FollowSwitch);
        }

        private void FollowSwitch()
        {
            try
            {
                if (whileOn.IsOn && !_detour.IsApplied)
                    _detour.Apply();
                else if (!whileOn.IsOn && _detour.IsApplied)
                    _detour.Undo();
            }
            catch (Exception exception)
            {
                LogFailure(exception);
            }
        }

        private void LogFailure(Exception exception) => RiskyCohesionPlugin.Log.LogError($"Failed to hook {type.FullName}.{methodName}: {exception}");
    }
}
