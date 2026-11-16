using Kofschip.Core.Conjugation;
using Kofschip.Core.Model;

namespace Kofschip.Tests.Conjugation;

public class ImperfectumTests
{
    private static readonly ConjugationEngine Engine = new();

    private static string Past(string infinitive, Person person = Person.Ik) =>
        Engine.Conjugate(new Verb { Infinitive = infinitive }, Tense.Imperfectum, person).Form;

    [Theory]
    [InlineData("werken", "werkte")]
    [InlineData("maken", "maakte")]
    [InlineData("fietsen", "fietste")]
    [InlineData("hopen", "hoopte")]
    [InlineData("douchen", "douchte")]
    [InlineData("missen", "miste")]
    [InlineData("stoppen", "stopte")]
    public void Letter_in_kofschip_gives_te(string infinitive, string expected) =>
        Assert.Equal(expected, Past(infinitive));

    [Theory]
    [InlineData("wonen", "woonde")]
    [InlineData("leren", "leerde")]
    [InlineData("bellen", "belde")]
    [InlineData("zorgen", "zorgde")]
    [InlineData("bouwen", "bouwde")]
    [InlineData("gooien", "gooide")]
    [InlineData("wandelen", "wandelde")]
    public void Letter_outside_kofschip_gives_de(string infinitive, string expected) =>
        Assert.Equal(expected, Past(infinitive));

    [Theory]
    [InlineData("leven", "leefde")]
    [InlineData("geloven", "geloofde")]
    [InlineData("durven", "durfde")]
    [InlineData("reizen", "reisde")]
    [InlineData("verhuizen", "verhuisde")]
    public void Kofschip_looks_at_the_infinitive_not_at_the_stem(string infinitive, string expected) =>
        Assert.Equal(expected, Past(infinitive));

    [Theory]
    [InlineData("praten", "praatte")]
    [InlineData("wachten", "wachtte")]
    [InlineData("zetten", "zette")]
    [InlineData("kosten", "kostte")]
    [InlineData("ontmoeten", "ontmoette")]
    [InlineData("antwoorden", "antwoordde")]
    [InlineData("landen", "landde")]
    [InlineData("redden", "redde")]
    [InlineData("leiden", "leidde")]
    public void Stem_on_t_or_d_keeps_its_letter(string infinitive, string expected) =>
        Assert.Equal(expected, Past(infinitive));

    [Theory]
    [InlineData(Person.Ik, "werkte")]
    [InlineData(Person.Jij, "werkte")]
    [InlineData(Person.U, "werkte")]
    [InlineData(Person.Hij, "werkte")]
    [InlineData(Person.Wij, "werkten")]
    [InlineData(Person.Jullie, "werkten")]
    [InlineData(Person.Zij, "werkten")]
    public void Plural_adds_n(Person person, string expected) =>
        Assert.Equal(expected, Past("werken", person));

    [Theory]
    [InlineData("wonen", "woonden")]
    [InlineData("praten", "praatten")]
    [InlineData("landen", "landden")]
    [InlineData("leven", "leefden")]
    public void Plural_forms(string infinitive, string expected) =>
        Assert.Equal(expected, Past(infinitive, Person.Wij));
}
