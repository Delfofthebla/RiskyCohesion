using System;
using System.Collections.Generic;

namespace RiskyCohesion;

internal static class Failsafe
{
    private static readonly HashSet<string> _reportedFailures = [];

    public static void Run(string task, Action action)
    {
        try
        {
            action();
        }
        catch (Exception exception)
        {
            Report(task, exception);
        }
    }

    private static void Report(string task, Exception exception)
    {
        if (_reportedFailures.Add(task + exception.GetType().FullName))
            RiskyCohesionPlugin.Log.LogError($"Failed {task}: {exception}");
    }
}
