using Kofschip.Core.Conjugation;
using Kofschip.Core.Model;

namespace Kofschip.Tests.Conjugation;

public class ParticipleTests
{
    private static readonly ConjugationEngine Engine = new();

    private static string Participle(string infinitive, string? stem = null, bool? prefix = null) =>
        Engine.Conjugate(
            new Verb { Infinitive = infinitive, Stem = stem, HasInseparablePrefix = prefix },
            Tense.Perfectum, Person.Ik).Form;

    [Theory]
    [InlineData("werken", "gewerkt")]
    [InlineData("maken", "gemaakt")]
    [InlineData("fietsen", "gefietst")]
    [InlineData("wonen", "gewoond")]
    [InlineData("leren", "geleerd")]
    [InlineData("leven", "geleefd")]
    [InlineData("reizen", "gereisd")]
    [InlineData("wandelen", "gewandeld")]
    public void Regular_participle_is_ge_stem_t_or_d(string infinitive, string expected) =>
        Assert.Equal(expected, Participle(infinitive));

    [Theory]
    [InlineData("praten", "gepraat")]
    [InlineData("wachten", "gewacht")]
    [InlineData("zetten", "gezet")]
    [InlineData("antwoorden", "geantwoord")]
    [InlineData("landen", "geland")]
    [InlineData("redden", "gered")]
    public void Stem_on_t_or_d_gets_no_extra_letter(string infinitive, string expected) =>
        Assert.Equal(expected, Participle(infinitive));

    [Theory]
    [InlineData("betalen", "betaald")]
    [InlineData("bedanken", "bedankt")]
    [InlineData("vertellen", "verteld")]
    [InlineData("verhuizen", "verhuisd")]
    [InlineData("gebruiken", "gebruikt")]
    [InlineData("geloven", "geloofd")]
    [InlineData("gebeuren", "gebeurd")]
    [InlineData("ontmoeten", "ontmoet")]
    [InlineData("ontdekken", "ontdekt")]
    [InlineData("herhalen", "herhaald")]
    [InlineData("herinneren", "herinnerd")]
    [InlineData("erkennen", "erkend")]
    public void Inseparable_prefix_means_no_ge(string infinitive, string expected) =>
        Assert.Equal(expected, Participle(infinitive));

    [Theory]
    [InlineData("bellen", "gebeld")]
    [InlineData("beven", "gebeefd")]
    [InlineData("verven", "geverfd")]
    [InlineData("erven", "geërfd")]
    public void Verbs_that_only_look_prefixed_still_get_ge(string infinitive, string expected) =>
        Assert.Equal(expected, Participle(infinitive));

    [Theory]
    [InlineData("eindigen", "geëindigd")]
    [InlineData("eisen", "geëist")]
    [InlineData("eren", "geëerd")]
    public void Trema_after_ge_before_e(string infinitive, string expected) =>
        Assert.Equal(expected, Participle(infinitive));

    [Fact]
    public void Trema_after_ge_before_i() =>
        Assert.Equal("geïnformeerd", Participle("informeren", stem: "informeer"));

    [Theory]
    [InlineData("oefenen", "geoefend")]
    [InlineData("openen", "geopend")]
    [InlineData("ademen", "geademd")]
    public void No_trema_before_other_vowels(string infinitive, string expected) =>
        Assert.Equal(expected, Participle(infinitive));

    [Theory]
    [InlineData("proberen", "probeer", "geprobeerd")]
    [InlineData("studeren", "studeer", "gestudeerd")]
    [InlineData("organiseren", "organiseer", "georganiseerd")]
    public void Uses_stem_from_data(string infinitive, string stem, string expected) =>
        Assert.Equal(expected, Participle(infinitive, stem));

    [Fact]
    public void Prefix_detection_can_be_switched_off_in_the_data() =>
        Assert.Equal("gebedeld", Participle("bedelen", prefix: false));

    [Fact]
    public void Prefix_can_be_declared_in_the_data() =>
        Assert.Equal("mislukt", Participle("mislukken", prefix: true));

    [Theory]
    [InlineData(Person.Ik)]
    [InlineData(Person.Hij)]
    [InlineData(Person.Wij)]
    public void Participle_does_not_depend_on_person(Person person) =>
        Assert.Equal("gewerkt",
            Engine.Conjugate(new Verb { Infinitive = "werken" }, Tense.Perfectum, person).Form);
}
