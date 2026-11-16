using Kofschip.Core.Conjugation;
using Kofschip.Core.Model;
using Kofschip.Core.Training;
using Kofschip.Data.Seed;

namespace Kofschip.Tests.Data;

/// <summary>Checks the shipped seed file, and the engine against every verb in it.</summary>
public class SeedDataTests
{
    private static readonly IReadOnlyList<Verb> Verbs = SeedReader.ReadVerbs();
    private static readonly ConjugationEngine Engine = new();

    public static IEnumerable<object[]> ExpectedVerbForms() =>
        ExpectedForms.All.Select(f => new object[] { f.Infinitive, f.Past, f.Participle });

    [Fact]
    public void Seed_has_at_least_100_verbs() =>
        Assert.True(Verbs.Count >= 100, $"Only {Verbs.Count} verbs in the seed file.");

    [Fact]
    public void Infinitives_are_unique() =>
        Assert.Equal(Verbs.Count, Verbs.Select(v => v.Infinitive).Distinct().Count());

    [Fact]
    public void Every_verb_has_a_sentence_for_each_tense()
    {
        foreach (var verb in Verbs)
        {
            foreach (var tense in Enum.GetValues<Tense>())
            {
                Assert.True(verb.Sentences.Any(s => s.Tense == tense),
                    $"'{verb.Infinitive}' has no sentence for {tense}.");
            }
        }
    }

    [Fact]
    public void Every_sentence_has_one_blank_and_ends_with_punctuation()
    {
        foreach (var sentence in Verbs.SelectMany(v => v.Sentences))
        {
            Assert.True(sentence.HasSingleBlank, sentence.Text);
            Assert.True(sentence.Text[^1] is '.' or '?' or '!', sentence.Text);
            Assert.Same(sentence.Verb, Verbs.Single(v => v.Infinitive == sentence.Verb.Infinitive));
        }
    }

    [Fact]
    public void Every_verb_has_expected_forms_to_check_against()
    {
        var expected = ExpectedForms.All.Select(f => f.Infinitive).ToHashSet();
        var missing = Verbs.Select(v => v.Infinitive).Where(i => !expected.Contains(i)).ToList();

        Assert.True(missing.Count == 0, "No expected forms for: " + string.Join(", ", missing));
    }

    [Theory]
    [MemberData(nameof(ExpectedVerbForms))]
    public void Engine_reproduces_the_known_forms(string infinitive, string past, string participle)
    {
        var verb = Verbs.Single(v => v.Infinitive == infinitive);

        Assert.Equal(past, Engine.Conjugate(verb, Tense.Imperfectum, Person.Ik).Form);
        Assert.Equal(past + "n", Engine.Conjugate(verb, Tense.Imperfectum, Person.Wij).Form);
        Assert.Equal(participle, Engine.Conjugate(verb, Tense.Perfectum, Person.Ik).Form);
    }

    [Fact]
    public void Every_sentence_can_be_turned_into_an_exercise()
    {
        foreach (var sentence in Verbs.SelectMany(v => v.Sentences))
        {
            var form = Engine.Conjugate(sentence.Verb, sentence.Tense, sentence.Person, sentence.Inversion);
            var exercise = new Exercise(sentence, form);

            Assert.False(string.IsNullOrWhiteSpace(exercise.Answer));
            Assert.DoesNotContain(Sentence.Blank, exercise.Solution);
        }
    }

    [Fact]
    public void Inversion_is_only_marked_for_jij_in_the_present_tense()
    {
        foreach (var sentence in Verbs.SelectMany(v => v.Sentences).Where(s => s.Inversion))
        {
            Assert.Equal(Person.Jij, sentence.Person);
            Assert.Equal(Tense.Presens, sentence.Tense);
        }
    }

    [Fact]
    public void Reader_rejects_a_sentence_without_blank()
    {
        const string json = """
            [{ "infinitive": "werken", "sentences": [{ "tense": "Presens", "person": "Ik", "text": "Ik werk." }] }]
            """;

        Assert.Throws<InvalidDataException>(() => SeedReader.ReadVerbs(ToStream(json)));
    }

    [Fact]
    public void Reader_rejects_duplicate_verbs()
    {
        const string json = """
            [{ "infinitive": "werken", "sentences": [] }, { "infinitive": "Werken", "sentences": [] }]
            """;

        Assert.Throws<InvalidDataException>(() => SeedReader.ReadVerbs(ToStream(json)));
    }

    [Fact]
    public void Reader_maps_optional_fields()
    {
        const string json = """
            [{
              "infinitive": "Proberen", "stem": "probeer", "prefix": false,
              "sentences": [{ "tense": "Presens", "person": "Jij", "inversion": true, "text": "___ jij het?" }]
            }]
            """;

        var verb = Assert.Single(SeedReader.ReadVerbs(ToStream(json)));
        var sentence = Assert.Single(verb.Sentences);

        Assert.Equal("proberen", verb.Infinitive);
        Assert.Equal("probeer", verb.Stem);
        Assert.False(verb.HasInseparablePrefix);
        Assert.True(sentence.Inversion);
        Assert.Equal(Person.Jij, sentence.Person);
        Assert.Same(verb, sentence.Verb);
    }

    private static MemoryStream ToStream(string text) => new(System.Text.Encoding.UTF8.GetBytes(text));
}
