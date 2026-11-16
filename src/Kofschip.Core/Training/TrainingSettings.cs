using Kofschip.Core.Model;

namespace Kofschip.Core.Training;

/// <summary>What the learner chose on the start page: which tenses and how many exercises.</summary>
public sealed class TrainingSettings
{
    public static readonly IReadOnlyList<int> AllowedLengths = [10, 20, 30];

    public const int DefaultLength = 10;

    public TrainingSettings(IEnumerable<Tense> tenses, int length = DefaultLength)
    {
        ArgumentNullException.ThrowIfNull(tenses);

        Tenses = tenses.Where(Enum.IsDefined).Distinct().Order().ToList();
        Length = AllowedLengths.Contains(length) ? length : DefaultLength;
    }

    /// <summary>All tenses, shortest session.</summary>
    public static TrainingSettings Default => new(Enum.GetValues<Tense>());

    public IReadOnlyList<Tense> Tenses { get; }

    public int Length { get; }

    /// <summary>A training needs at least one tense.</summary>
    public bool CanStart => Tenses.Count > 0;

    /// <summary>Query string for the training page, e.g. "tijden=presens,perfectum&amp;aantal=20".</summary>
    public string ToQueryString() =>
        $"tijden={string.Join(',', Tenses.Select(t => t.ToString().ToLowerInvariant()))}&aantal={Length}";

    /// <summary>Reads the values of <see cref="ToQueryString"/>; unknown values are ignored.</summary>
    public static TrainingSettings Parse(string? tenses, int? length)
    {
        var parsed = (tenses ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(name => Enum.TryParse<Tense>(name, ignoreCase: true, out var tense) && !int.TryParse(name, out _)
                ? tense
                : (Tense?)null)
            .Where(tense => tense.HasValue)
            .Select(tense => tense!.Value);

        return new TrainingSettings(parsed, length ?? DefaultLength);
    }
}
