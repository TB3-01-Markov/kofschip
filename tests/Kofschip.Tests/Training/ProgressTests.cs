using System.Text.Json;
using Kofschip.Core.Model;
using Kofschip.Core.Training;

namespace Kofschip.Tests.Training;

public class ProgressTests
{
    [Fact]
    public void New_progress_is_empty()
    {
        var progress = new Progress();

        Assert.True(progress.IsEmpty);
        Assert.Null(progress.For(Tense.Presens).Percentage);
        Assert.Equal(0, progress.For(Tense.Presens).Attempts);
    }

    [Fact]
    public void Records_attempts_per_tense()
    {
        var progress = new Progress();

        progress.Record(Tense.Imperfectum, correct: true);
        progress.Record(Tense.Imperfectum, correct: false);
        progress.Record(Tense.Imperfectum, correct: true);
        progress.Record(Tense.Perfectum, correct: false);

        Assert.False(progress.IsEmpty);
        Assert.Equal(3, progress.For(Tense.Imperfectum).Attempts);
        Assert.Equal(2, progress.For(Tense.Imperfectum).Correct);
        Assert.Equal(67, progress.For(Tense.Imperfectum).Percentage);
        Assert.Equal(0, progress.For(Tense.Perfectum).Percentage);
        Assert.Null(progress.For(Tense.Presens).Percentage);
    }

    [Fact]
    public void Survives_a_json_round_trip()
    {
        var progress = new Progress();
        progress.Record(Tense.Presens, correct: true);
        progress.Record(Tense.Perfectum, correct: false);

        var restored = JsonSerializer.Deserialize<Progress>(JsonSerializer.Serialize(progress))!;

        Assert.Equal(1, restored.For(Tense.Presens).Correct);
        Assert.Equal(1, restored.For(Tense.Perfectum).Attempts);
        Assert.Equal(0, restored.For(Tense.Perfectum).Correct);
    }
}
