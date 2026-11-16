using Kofschip.Core.Conjugation;
using Kofschip.Core.Model;
using Kofschip.Core.Training;

namespace Kofschip.Tests.Training;

/// <summary>Small builders shared by the training tests.</summary>
internal static class TestData
{
    private static readonly ConjugationEngine Engine = new();

    public static Sentence Sentence(
        string infinitive, Tense tense, string text, Person person = Person.Ik, bool inversion = false) =>
        new()
        {
            Verb = new Verb { Infinitive = infinitive },
            Tense = tense,
            Person = person,
            Inversion = inversion,
            Text = text,
        };

    public static Exercise Exercise(
        string infinitive, Tense tense, string text, Person person = Person.Ik, bool inversion = false)
    {
        var sentence = Sentence(infinitive, tense, text, person, inversion);
        return new Exercise(sentence, Engine.Conjugate(sentence.Verb, tense, person, inversion));
    }

    /// <summary>werkte, woonde, gemaakt.</summary>
    public static List<Exercise> ThreeExercises() =>
    [
        Exercise("werken", Tense.Imperfectum, "Gisteren ___ ik de hele dag."),
        Exercise("wonen", Tense.Imperfectum, "Vroeger ___ ik in Gent."),
        Exercise("maken", Tense.Perfectum, "Ik heb soep ___."),
    ];
}
