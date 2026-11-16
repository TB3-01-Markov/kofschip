using Kofschip.Core.Training;

namespace Kofschip.Tests.Training;

public class AnswerCheckerTests
{
    [Theory]
    [InlineData("werkte")]
    [InlineData("Werkte")]
    [InlineData("WERKTE")]
    [InlineData("  werkte ")]
    [InlineData("werkte\n")]
    public void Case_and_surrounding_whitespace_are_ignored(string given) =>
        Assert.True(AnswerChecker.IsCorrect(given, "werkte"));

    [Theory]
    [InlineData("werkde")]
    [InlineData("werkt")]
    [InlineData("werk te")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Wrong_or_missing_answers_are_rejected(string? given) =>
        Assert.False(AnswerChecker.IsCorrect(given, "werkte"));

    [Fact]
    public void Trema_is_part_of_the_spelling()
    {
        Assert.True(AnswerChecker.IsCorrect("geëindigd", "geëindigd"));
        Assert.False(AnswerChecker.IsCorrect("geeindigd", "geëindigd"));
    }

    [Fact]
    public void Decomposed_trema_equals_composed_trema() =>
        Assert.True(AnswerChecker.IsCorrect("geëindigd", "geëindigd"));

    [Fact]
    public void Capitalised_expected_form_matches_lower_case_answer() =>
        Assert.True(AnswerChecker.IsCorrect("maak", "Maak"));
}
