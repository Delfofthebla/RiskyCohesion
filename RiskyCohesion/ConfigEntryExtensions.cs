using System;
using BepInEx.Configuration;

namespace RiskyCohesion;

internal static class ConfigEntryExtensions
{
    public static void ApplyNowAndOnChange<T>(this ConfigEntry<T> entry, Action apply)
    {
        apply();
        entry.SettingChanged += (_, _) => apply();
    }
}
