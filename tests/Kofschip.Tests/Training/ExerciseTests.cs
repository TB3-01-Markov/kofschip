using Kofschip.Core.Conjugation;
using Kofschip.Core.Model;
using Kofschip.Core.Training;

namespace Kofschip.Tests.Training;

public class ExerciseTests
{
    [Fact]
    public void Splits_the_sentence_around_the_blank()
    {
        var exercise = TestData.Exercise("werken", Tense.Imperfectum, "Gisteren ___ ik de hele dag.");

        Assert.Equal("Gisteren ", exercise.TextBefore);
        Assert.Equal(" ik de hele dag.", exercise.TextAfter);
        Assert.Equal("werkte", exercise.Answer);
        Assert.Equal("Gisteren werkte ik de hele dag.", exercise.Solution);
        Assert.Equal("werken", exercise.Infinitive);
        Assert.Equal(Tense.Imperfectum, exercise.Tense);
    }

    [Fact]
    public void Blank_at_the_start_gets_a_capital_letter()
    {
        var exercise = TestData.Exercise(
            "maken", Tense.Presens, "___ jij vanavond het eten?", Person.Jij, inversion: true);

        Assert.Equal("Maak", exercise.Answer);
        Assert.Equal("Maak jij vanavond het eten?", exercise.Solution);
    }

    [Theory]
    [InlineData("Geen gat in deze zin.")]
    [InlineData("Twee ___ gaten ___ hier.")]
    public void Sentence_needs_exactly_one_blank(string text)
    {
        var sentence = TestData.Sentence("werken", Tense.Presens, text);

        Assert.False(sentence.HasSingleBlank);
        Assert.Throws<InvalidOperationException>(
            () => new Exercise(sentence, new ConjugatedForm("werk", [])));
    }
}
