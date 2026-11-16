using Kofschip.Core.Conjugation;
using Kofschip.Core.Model;

namespace Kofschip.Tests.Conjugation;

public class PresensTests
{
    private static readonly ConjugationEngine Engine = new();

    private static string Present(string infinitive, Person person, bool inversion = false) =>
        Engine.Conjugate(new Verb { Infinitive = infinitive }, Tense.Presens, person, inversion).Form;

    [Theory]
    [InlineData(Person.Ik, "werk")]
    [InlineData(Person.Jij, "werkt")]
    [InlineData(Person.U, "werkt")]
    [InlineData(Person.Hij, "werkt")]
    [InlineData(Person.Wij, "werken")]
    [InlineData(Person.Jullie, "werken")]
    [InlineData(Person.Zij, "werken")]
    public void Regular_forms(Person person, string expected) =>
        Assert.Equal(expected, Present("werken", person));

    [Fact]
    public void Jij_after_the_verb_loses_the_t() =>
        Assert.Equal("werk", Present("werken", Person.Jij, inversion: true));

    [Theory]
    [InlineData(Person.U)]
    [InlineData(Person.Hij)]
    public void Inversion_only_matters_for_jij(Person person) =>
        Assert.Equal("werkt", Present("werken", person, inversion: true));

    [Theory]
    [InlineData("praten", "praat")]
    [InlineData("wachten", "wacht")]
    [InlineData("zetten", "zet")]
    [InlineData("kosten", "kost")]
    public void Stem_on_t_gets_no_second_t(string infinitive, string expected) =>
        Assert.Equal(expected, Present(infinitive, Person.Hij));

    [Theory]
    [InlineData("antwoorden", "antwoordt")]
    [InlineData("landen", "landt")]
    [InlineData("redden", "redt")]
    [InlineData("leiden", "leidt")]
    public void Stem_on_d_still_gets_t(string infinitive, string expected) =>
        Assert.Equal(expected, Present(infinitive, Person.Hij));

    [Theory]
    [InlineData("leven", "leeft")]
    [InlineData("reizen", "reist")]
    [InlineData("maken", "maakt")]
    [InlineData("betalen", "betaalt")]
    public void Third_person_is_built_on_the_stem(string infinitive, string expected) =>
        Assert.Equal(expected, Present(infinitive, Person.Hij));

    [Theory]
    [InlineData("leven")]
    [InlineData("pakken")]
    [InlineData("betalen")]
    public void Plural_is_the_infinitive(string infinitive) =>
        Assert.Equal(infinitive, Present(infinitive, Person.Wij));
}
