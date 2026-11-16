using System.Text.Json.Serialization;
using Kofschip.Core.Model;

namespace Kofschip.Core.Training;

/// <summary>Attempts and correct answers for one tense.</summary>
public sealed class TenseProgress
{
    public int Attempts { get; set; }

    public int Correct { get; set; }

    /// <summary>Whole percent of correct answers, or null before the first attempt.</summary>
    [JsonIgnore]
    public int? Percentage => Attempts == 0
        ? null
        : (int)Math.Round(100.0 * Correct / Attempts, MidpointRounding.AwayFromZero);
}

/// <summary>
/// The learner's results per tense. A plain object so it can be stored as JSON
/// in the browser (no account needed).
/// </summary>
public sealed class Progress
{
    public Dictionary<Tense, TenseProgress> Tenses { get; set; } = [];

    public TenseProgress For(Tense tense) =>
        Tenses.TryGetValue(tense, out var progress) ? progress : new TenseProgress();

    public void Record(Tense tense, bool correct)
    {
        if (!Tenses.TryGetValue(tense, out var progress))
        {
            progress = new TenseProgress();
            Tenses[tense] = progress;
        }

        progress.Attempts++;
        if (correct)
        {
            progress.Correct++;
        }
    }

    [JsonIgnore]
    public bool IsEmpty => Tenses.Values.All(p => p.Attempts == 0);
}
