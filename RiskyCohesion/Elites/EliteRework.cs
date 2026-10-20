using System;
using RiskyCohesion.Sources;

namespace RiskyCohesion.Elites;

internal sealed class EliteRework(string requirement, Func<bool> isOn)
{
    public string Requirement { get; } = requirement;
    public bool IsOn => isOn();

    public static readonly EliteRework Blazing = new("EliteReworks' T1 - Blazing module", () => EliteReworksSource.ReworksBlazing);
    public static readonly EliteRework OverloadingLightning = new("EliteReworks' T1 - Overloading module and its Passive Lightning", () => EliteReworksSource.ReworksOverloadingLightning);
    public static readonly EliteRework Malachite = new("EliteReworks' T2 - Malachite module", () => EliteReworksSource.ReworksMalachite);
}
