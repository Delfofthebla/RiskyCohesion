using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RiskyCohesion;

internal sealed class Description(string summary)
{
    private readonly List<string> _references = [];

    private string _requirement;

    public static implicit operator string(Description description) => description.ToString();

    public static string Format(float value) => value.ToString("0.##", CultureInfo.InvariantCulture);

    public Description Requires(string requirement)
    {
        _requirement = $"Requires {requirement}.";
        return this;
    }

    public Description Reference(string source, string value)
    {
        _references.Add($"{source}: {value}");
        return this;
    }

    public Description Vanilla(string value) => Reference("Vanilla", value);

    public override string ToString()
    {
        string[][] paragraphs =
        [
            [summary, _requirement],
            [.. _references],
        ];

        return string.Join("\n\n", paragraphs.Select(Paragraph).Where(paragraph => paragraph.Length > 0));
    }

    private static string Paragraph(string[] lines) => string.Join("\n", lines.Where(line => !string.IsNullOrEmpty(line)));
}
