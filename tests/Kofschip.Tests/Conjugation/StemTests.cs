using Kofschip.Core.Conjugation;
using Kofschip.Core.Model;

namespace Kofschip.Tests.Conjugation;

/// <summary>The stem is the ik-form of the present tense, so that is what these tests ask for.</summary>
public class StemTests
{
    private static readonly ConjugationEngine Engine = new();

    private static string StemOf(string infinitive, string? stem = null) =>
        Engine.Conjugate(new Verb { Infinitive = infinitive, Stem = stem }, Tense.Presens, Person.Ik).Form;

    [Theory]
    [InlineData("werken", "werk")]
    [InlineData("fietsen", "fiets")]
    [InlineData("wachten", "wacht")]
    [InlineData("antwoorden", "antwoord")]
    [InlineData("zorgen", "zorg")]
    public void Stem_is_infinitive_without_en(string infinitive, string expected) =>
        Assert.Equal(expected, StemOf(infinitive));

    [Theory]
    [InlineData("maken", "maak")]
    [InlineData("leren", "leer")]
    [InlineData("wonen", "woon")]
    [InlineData("huren", "huur")]
    [InlineData("spelen", "speel")]
    [InlineData("koken", "kook")]
    [InlineData("praten", "praat")]
    public void Long_vowel_is_written_double(string infinitive, string expected) =>
        Assert.Equal(expected, StemOf(infinitive));

    [Theory]
    [InlineData("pakken", "pak")]
    [InlineData("bellen", "bel")]
    [InlineData("stoppen", "stop")]
    [InlineData("zetten", "zet")]
    [InlineData("kennen", "ken")]
    [InlineData("missen", "mis")]
    [InlineData("redden", "red")]
    public void Double_consonant_becomes_single(string infinitive, string expected) =>
        Assert.Equal(expected, StemOf(infinitive));

    [Theory]
    [InlineData("leven", "leef")]
    [InlineData("geloven", "geloof")]
    [InlineData("durven", "durf")]
    [InlineData("verven", "verf")]
    [InlineData("reizen", "reis")]
    [InlineData("verhuizen", "verhuis")]
    public void V_becomes_f_and_z_becomes_s(string infinitive, string expected) =>
        Assert.Equal(expected, StemOf(infinitive));

    [Theory]
    [InlineData("voelen", "voel")]
    [InlineData("huilen", "huil")]
    [InlineData("noemen", "noem")]
    [InlineData("bouwen", "bouw")]
    [InlineData("leiden", "leid")]
    [InlineData("verdienen", "verdien")]
    [InlineData("gooien", "gooi")]
    [InlineData("duwen", "duw")]
    public void Vowel_combinations_are_not_doubled(string infinitive, string expected) =>
        Assert.Equal(expected, StemOf(infinitive));

    [Theory]
    [InlineData("wandelen", "wandel")]
    [InlineData("luisteren", "luister")]
    [InlineData("openen", "open")]
    [InlineData("tekenen", "teken")]
    [InlineData("regenen", "regen")]
    [InlineData("oefenen", "oefen")]
    [InlineData("ademen", "adem")]
    [InlineData("veranderen", "verander")]
    [InlineData("herinneren", "herinner")]
    [InlineData("betekenen", "beteken")]
    [InlineData("verzamelen", "verzamel")]
    public void Unstressed_ending_is_not_doubled(string infinitive, string expected) =>
        Assert.Equal(expected, StemOf(infinitive));

    [Theory]
    [InlineData("betalen", "betaal")]
    [InlineData("herhalen", "herhaal")]
    [InlineData("vertalen", "vertaal")]
    [InlineData("beloven", "beloof")]
    [InlineData("verhuren", "verhuur")]
    public void Long_vowel_after_prefix_is_written_double(string infinitive, string expected) =>
        Assert.Equal(expected, StemOf(infinitive));

    [Theory]
    [InlineData("proberen", "probeer")]
    [InlineData("studeren", "studeer")]
    [InlineData("feliciteren", "feliciteer")]
    public void Stem_from_data_wins_over_the_rules(string infinitive, string stem) =>
        Assert.Equal(stem, StemOf(infinitive, stem));

    [Theory]
    [InlineData("gaan")]
    [InlineData("doen")]
    [InlineData("zien")]
    [InlineData("en")]
    [InlineData("")]
    public void Only_stem_plus_en_is_supported(string infinitive) =>
        Assert.Throws<ArgumentException>(() => StemOf(infinitive));

    [Fact]
    public void Infinitive_is_trimmed_and_lower_cased() =>
        Assert.Equal("werk", StemOf("  Werken "));
}
