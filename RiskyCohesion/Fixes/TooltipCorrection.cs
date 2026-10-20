using System;
using System.Text.RegularExpressions;
using RoR2;

namespace RiskyCohesion.Fixes;

internal sealed class TooltipCorrection(string token, float stated, float actual)
{
    private static readonly SharedHook<TooltipCorrection> _localizedStrings = new(typeof(Language), nameof(Language.GetLocalizedStringByToken), GetLocalizedStringByToken);

    private readonly Regex _statedNumber = new($@"(?<![\d.]){Regex.Escape(Description.Format(stated))}(?!\d)");
    private readonly string _actualNumber = Description.Format(actual);

    public void Hook(Rule rule) => _localizedStrings.Add(this, rule);

    private static string GetLocalizedStringByToken(Func<Language, string, string> orig, Language language, string requestedToken)
    {
        var text = orig(language, requestedToken);
        foreach (var correction in _localizedStrings.Active)
            text = correction.Correct(requestedToken, text);

        return text;
    }

    private string Correct(string requestedToken, string text) => requestedToken == token
        ? _statedNumber.Replace(text, _actualNumber, 1)
        : text;
}
