using Kofschip.Core.Model;

namespace Kofschip.Core.Conjugation;

internal sealed record StemResult(
    string Infinitive,
    string Stem,
    char LetterBeforeEn,
    string? Prefix,
    IReadOnlyList<ExplanationStep> Steps);

/// <summary>Derives the stem (ik-vorm) of a weak verb from its infinitive.</summary>
internal static class StemBuilder
{
    private const string Vowels = "aeiou";

    /// <summary>Unstressed prefixes that are never separated from the verb.</summary>
    private static readonly string[] InseparablePrefixes = ["ver", "ont", "her", "be", "ge", "er"];

    public static StemResult Build(Verb verb)
    {
        var infinitive = verb.Infinitive.Trim().ToLowerInvariant();
        if (!IsStemPlusEn(infinitive))
        {
            throw new ArgumentException(
                $"Only infinitives of the form stem + -en are supported, got \"{verb.Infinitive}\".", nameof(verb));
        }

        var raw = infinitive[..^2];
        var prefix = FindPrefix(verb, infinitive);
        var steps = new List<ExplanationStep>();
        string stem;

        if (!string.IsNullOrWhiteSpace(verb.Stem))
        {
            stem = verb.Stem.Trim().ToLowerInvariant();
            steps.Add(new(Rule.StemFromData, $"De stam van {infinitive} is {stem}."));
        }
        else
        {
            stem = raw;
            steps.Add(new(Rule.Stem, $"Haal -en weg: {infinitive} → {raw}."));

            if (EndsInDoubleConsonant(raw))
            {
                stem = raw[..^1];
                steps.Add(new(Rule.SingleConsonant,
                    $"Een stam eindigt niet op twee dezelfde medeklinkers: {raw} → {stem}."));
            }
            else if (NeedsDoubleVowel(raw[(prefix?.Length ?? 0)..]))
            {
                stem = $"{raw[..^1]}{raw[^2]}{raw[^1]}";
                steps.Add(new(Rule.LongVowel,
                    $"De klinker is lang, dus je schrijft hem dubbel: {raw} → {stem}."));
            }

            if (stem.EndsWith('v'))
            {
                var before = stem;
                stem = stem[..^1] + 'f';
                steps.Add(new(Rule.VBecomesF, $"Een stam eindigt niet op v: {before} → {stem}."));
            }
            else if (stem.EndsWith('z'))
            {
                var before = stem;
                stem = stem[..^1] + 's';
                steps.Add(new(Rule.ZBecomesS, $"Een stam eindigt niet op z: {before} → {stem}."));
            }
        }

        return new StemResult(infinitive, stem, raw[^1], prefix, steps);
    }

    /// <summary>
    /// Finds an inseparable prefix. The check on the remainder keeps verbs such as
    /// bellen and verven (no prefix) apart from betalen and vertellen.
    /// </summary>
    private static string? FindPrefix(Verb verb, string infinitive)
    {
        if (verb.HasInseparablePrefix == false)
        {
            return null;
        }

        foreach (var prefix in InseparablePrefixes)
        {
            if (!infinitive.StartsWith(prefix, StringComparison.Ordinal))
            {
                continue;
            }

            var remainder = infinitive[prefix.Length..^2];
            if (remainder.Length >= 2 && remainder.Any(IsVowel))
            {
                return prefix;
            }
        }

        return null;
    }

    /// <summary>
    /// Rejects infinitives that are stem + -n (doen, gaan, zien): those verbs are irregular.
    /// What precedes -en must end in a consonant or in a vowel + i (gooien, zwaaien).
    /// </summary>
    private static bool IsStemPlusEn(string infinitive)
    {
        if (infinitive.Length < 4 || !infinitive.EndsWith("en", StringComparison.Ordinal))
        {
            return false;
        }

        char last = infinitive[^3], beforeLast = infinitive[^4];
        return !IsVowel(last) || (last == 'i' && IsVowel(beforeLast));
    }

    private static bool EndsInDoubleConsonant(string raw) =>
        raw.Length >= 2 && raw[^1] == raw[^2] && !IsVowel(raw[^1]);

    /// <summary>
    /// True when the last syllable has a long vowel that the infinitive writes with one
    /// letter (ma-ken) and the stem must write with two (maak).
    /// </summary>
    /// <param name="core">Infinitive without -en and without inseparable prefix.</param>
    private static bool NeedsDoubleVowel(string core)
    {
        if (core.Length < 2)
        {
            return false;
        }

        char consonant = core[^1], vowel = core[^2];

        // Only a, e, o and u are doubled, and only before a single final consonant.
        if (IsVowel(consonant) || consonant == 'x' || !"aeou".Contains(vowel))
        {
            return false;
        }

        // Part of a vowel combination (oe, ie, eu, ui, ou, aa ...): already long.
        if (core.Length >= 3 && IsVowel(core[^3]))
        {
            return false;
        }

        // duwen → duw.
        if (vowel == 'u' && consonant == 'w')
        {
            return false;
        }

        // Unstressed -el, -er, -en, -em after another syllable: wandelen → wandel, openen → open.
        // Verbs with a stressed ending (proberen → probeer) carry their stem in the data.
        if (vowel == 'e' && "lrnm".Contains(consonant) && core[..^2].Any(IsVowel))
        {
            return false;
        }

        return true;
    }

    internal static bool IsVowel(char c) => Vowels.Contains(c);
}
