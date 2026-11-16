using System.Text.Json;
using System.Text.Json.Serialization;
using Kofschip.Core.Model;

namespace Kofschip.Data.Seed;

/// <summary>Reads the verbs and example sentences shipped with the application.</summary>
public static class SeedReader
{
    private const string ResourceName = "Kofschip.Data.Seed.verbs.json";

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    /// <summary>Reads the embedded seed file.</summary>
    public static IReadOnlyList<Verb> ReadVerbs()
    {
        using var stream = typeof(SeedReader).Assembly.GetManifestResourceStream(ResourceName)
            ?? throw new InvalidOperationException($"Embedded resource '{ResourceName}' not found.");
        return ReadVerbs(stream);
    }

    /// <summary>Reads and validates seed data from JSON.</summary>
    public static IReadOnlyList<Verb> ReadVerbs(Stream json)
    {
        var seeds = JsonSerializer.Deserialize<List<VerbSeed>>(json, Options)
            ?? throw new InvalidDataException("The seed file is empty.");

        var verbs = new List<Verb>(seeds.Count);
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var seed in seeds)
        {
            if (string.IsNullOrWhiteSpace(seed.Infinitive))
            {
                throw new InvalidDataException("A verb in the seed file has no infinitive.");
            }

            if (!seen.Add(seed.Infinitive))
            {
                throw new InvalidDataException($"Verb '{seed.Infinitive}' appears twice in the seed file.");
            }

            var verb = new Verb
            {
                Infinitive = seed.Infinitive.Trim().ToLowerInvariant(),
                Stem = string.IsNullOrWhiteSpace(seed.Stem) ? null : seed.Stem.Trim().ToLowerInvariant(),
                HasInseparablePrefix = seed.Prefix,
            };

            foreach (var s in seed.Sentences ?? [])
            {
                var sentence = new Sentence
                {
                    Verb = verb,
                    Tense = s.Tense,
                    Person = s.Person,
                    Inversion = s.Inversion,
                    Text = s.Text?.Trim() ?? string.Empty,
                };

                if (!sentence.HasSingleBlank)
                {
                    throw new InvalidDataException(
                        $"Sentence for '{verb.Infinitive}' must contain '{Sentence.Blank}' exactly once: \"{s.Text}\"");
                }

                verb.Sentences.Add(sentence);
            }

            verbs.Add(verb);
        }

        return verbs;
    }

    private sealed record VerbSeed(string Infinitive, string? Stem, bool? Prefix, List<SentenceSeed>? Sentences);

    private sealed record SentenceSeed(Tense Tense, Person Person, string? Text, bool Inversion = false);
}
