using Kofschip.Core.Model;
using Kofschip.Core.Training;

namespace Kofschip.Tests.Training;

public class TrainingSettingsTests
{
    [Fact]
    public void Default_is_all_tenses_and_ten_exercises()
    {
        var settings = TrainingSettings.Default;

        Assert.Equal(new[] { Tense.Presens, Tense.Imperfectum, Tense.Perfectum }, settings.Tenses);
        Assert.Equal(10, settings.Length);
        Assert.True(settings.CanStart);
    }

    [Fact]
    public void Training_cannot_start_without_a_tense() =>
        Assert.False(new TrainingSettings([]).CanStart);

    [Theory]
    [InlineData(10, 10)]
    [InlineData(20, 20)]
    [InlineData(30, 30)]
    [InlineData(15, 10)]
    [InlineData(0, 10)]
    [InlineData(-5, 10)]
    [InlineData(1000, 10)]
    public void Only_allowed_lengths_are_kept(int requested, int expected) =>
        Assert.Equal(expected, new TrainingSettings([Tense.Presens], requested).Length);

    [Fact]
    public void Duplicate_tenses_are_removed_and_sorted()
    {
        var settings = new TrainingSettings([Tense.Perfectum, Tense.Presens, Tense.Perfectum]);

        Assert.Equal(new[] { Tense.Presens, Tense.Perfectum }, settings.Tenses);
    }

    [Fact]
    public void Query_string_round_trips()
    {
        var settings = new TrainingSettings([Tense.Imperfectum, Tense.Perfectum], 20);

        Assert.Equal("tijden=imperfectum,perfectum&aantal=20", settings.ToQueryString());

        var parsed = TrainingSettings.Parse("imperfectum,perfectum", 20);
        Assert.Equal(settings.Tenses, parsed.Tenses);
        Assert.Equal(20, parsed.Length);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("futurum")]
    [InlineData("1")]
    [InlineData(",,")]
    public void Unknown_tenses_are_ignored(string? tenses)
    {
        var parsed = TrainingSettings.Parse(tenses, null);

        Assert.Empty(parsed.Tenses);
        Assert.Equal(10, parsed.Length);
    }

    [Fact]
    public void Parsing_is_case_insensitive_and_trims() =>
        Assert.Equal(
            new[] { Tense.Presens, Tense.Perfectum },
            TrainingSettings.Parse(" Perfectum , PRESENS ", 30).Tenses);
}
