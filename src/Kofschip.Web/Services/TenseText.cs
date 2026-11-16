using Kofschip.Core.Model;

namespace Kofschip.Web.Services;

/// <summary>Dutch names and examples for the tenses, as shown in the interface.</summary>
public static class TenseText
{
    public static string Name(Tense tense) => tense switch
    {
        Tense.Presens => "Presens",
        Tense.Imperfectum => "Imperfectum",
        Tense.Perfectum => "Perfectum",
        _ => tense.ToString(),
    };

    public static string Description(Tense tense) => tense switch
    {
        Tense.Presens => "tegenwoordige tijd",
        Tense.Imperfectum => "verleden tijd",
        Tense.Perfectum => "voltooide tijd",
        _ => string.Empty,
    };

    public static string Example(Tense tense) => tense switch
    {
        Tense.Presens => "ik werk",
        Tense.Imperfectum => "ik werkte",
        Tense.Perfectum => "ik heb gewerkt",
        _ => string.Empty,
    };
}
