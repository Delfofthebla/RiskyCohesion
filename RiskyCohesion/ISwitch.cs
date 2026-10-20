using System;

namespace RiskyCohesion;

internal interface ISwitch
{
    bool IsOn { get; }

    void ApplyNowAndOnChange(Action apply);
}
