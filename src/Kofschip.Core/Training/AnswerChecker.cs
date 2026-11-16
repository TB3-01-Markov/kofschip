using System.Text;

namespace Kofschip.Core.Training;

/// <summary>Compares a typed answer with the expected form.</summary>
public static class AnswerChecker
{
    /// <summary>
    /// Case and surrounding whitespace do not matter; spelling does, including the trema.
    /// </summary>
    public static bool IsCorrect(string? given, string expected) =>
        Normalize(given) == Normalize(expected);

    internal static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        // FormC makes "e" + combining diaeresis equal to "ë".
        var words = value.Normalize(NormalizationForm.FormC)
            .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        return string.Join(' ', words).ToLowerInvariant();
    }
}
