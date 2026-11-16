using Kofschip.Core.Conjugation;
using Kofschip.Core.Model;

namespace Kofschip.Tests.Conjugation;

public class ExplanationTests
{
    private static readonly ConjugationEngine Engine = new();

    private static ConjugatedForm Conjugate(string infinitive, Tense tense, Person person = Person.Ik) =>
        Engine.Conjugate(new Verb { Infinitive = infinitive }, tense, person);

    private static Rule[] Rules(ConjugatedForm form) => form.Steps.Select(s => s.Rule).ToArray();

    [Fact]
    public void Simple_past_lists_stem_kofschip_and_ending()
    {
        var form = Conjugate("werken", Tense.Imperfectum);

        Assert.Equal(new[] { Rule.Stem, Rule.Kofschip, Rule.PastSingular }, Rules(form));
        Assert.Equal('k', form.KofschipLetter);
        Assert.Contains("'t kofschip", form.Steps[1].Text);
        Assert.Contains("werk + te → werkte", form.Steps[2].Text);
    }

    [Fact]
    public void Leven_explains_every_spelling_change()
    {
        var form = Conjugate("leven", Tense.Imperfectum);

        Assert.Equal(new[] { Rule.Stem, Rule.LongVowel, Rule.VBecomesF, Rule.NotKofschip, Rule.PastSingular }, Rules(form));
        Assert.Equal('v', form.KofschipLetter);
    }

    [Fact]
    public void Plural_past_is_marked_as_plural() =>
        Assert.Contains(Rule.PastPlural, Rules(Conjugate("werken", Tense.Imperfectum, Person.Wij)));

    [Fact]
    public void Participle_with_prefix_explains_missing_ge()
    {
        var form = Conjugate("betalen", Tense.Perfectum);

        Assert.Contains(Rule.ParticipleWithoutGe, Rules(form));
        Assert.Contains(form.Steps, s => s.Text.Contains("be-"));
    }

    [Fact]
    public void Participle_explains_missing_extra_t() =>
        Assert.Contains(Rule.ParticipleNoExtraEnding, Rules(Conjugate("praten", Tense.Perfectum)));

    [Fact]
    public void Participle_explains_trema() =>
        Assert.Contains(Rule.ParticipleTrema, Rules(Conjugate("eindigen", Tense.Perfectum)));

    [Fact]
    public void Present_tense_does_not_mention_kofschip()
    {
        var form = Conjugate("werken", Tense.Presens, Person.Hij);

        Assert.Null(form.KofschipLetter);
        Assert.Equal(new[] { Rule.Stem, Rule.PresensStemPlusT }, Rules(form));
    }

    [Fact]
    public void Plural_present_needs_one_step() =>
        Assert.Equal(new[] { Rule.PresensPlural }, Rules(Conjugate("werken", Tense.Presens, Person.Wij)));

    [Fact]
    public void Last_step_always_shows_the_form()
    {
        foreach (var tense in Enum.GetValues<Tense>())
        {
            foreach (var person in Enum.GetValues<Person>())
            {
                var form = Conjugate("leven", tense, person);
                Assert.Contains(form.Form, form.Steps[^1].Text);
            }
        }
    }

    [Theory]
    [InlineData('t', true)]
    [InlineData('k', true)]
    [InlineData('f', true)]
    [InlineData('s', true)]
    [InlineData('c', true)]
    [InlineData('h', true)]
    [InlineData('p', true)]
    [InlineData('x', true)]
    [InlineData('K', true)]
    [InlineData('d', false)]
    [InlineData('v', false)]
    [InlineData('z', false)]
    [InlineData('n', false)]
    [InlineData('l', false)]
    public void Kofschip_letters(char letter, bool expected) =>
        Assert.Equal(expected, ConjugationEngine.IsInKofschip(letter));
}
