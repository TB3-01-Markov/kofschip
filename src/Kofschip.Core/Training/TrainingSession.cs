namespace Kofschip.Core.Training;

/// <summary>
/// One training round: a fixed list of exercises that are answered in order.
/// Holds no UI or storage concerns, so it can be unit tested on its own.
/// </summary>
public sealed class TrainingSession
{
    private readonly IReadOnlyList<Exercise> _exercises;
    private readonly List<AnswerResult> _results = [];
    private int _index;

    public TrainingSession(IEnumerable<Exercise> exercises)
    {
        ArgumentNullException.ThrowIfNull(exercises);

        _exercises = exercises.ToList();
        if (_exercises.Count == 0)
        {
            throw new ArgumentException("A session needs at least one exercise.", nameof(exercises));
        }
    }

    public int Total => _exercises.Count;

    /// <summary>1-based number of the current exercise, for the "7 / 10" counter.</summary>
    public int Position => Math.Min(_index + 1, Total);

    public bool IsFinished => _index >= Total;

    /// <summary>True when the current exercise has been answered but not yet left.</summary>
    public bool IsAnswered => !IsFinished && _results.Count > _index;

    public Exercise Current => IsFinished
        ? throw new InvalidOperationException("The session is finished.")
        : _exercises[_index];

    /// <summary>The result of the current exercise, or null while it is unanswered.</summary>
    public AnswerResult? CurrentResult => IsAnswered ? _results[_index] : null;

    public IReadOnlyList<AnswerResult> Results => _results;

    public int CorrectCount => _results.Count(r => r.IsCorrect);

    /// <summary>Share of correct answers among all exercises, rounded to a whole percent.</summary>
    public int Percentage => (int)Math.Round(100.0 * CorrectCount / Total, MidpointRounding.AwayFromZero);

    public IReadOnlyList<AnswerResult> Mistakes => _results.Where(r => !r.IsCorrect).ToList();

    /// <summary>Checks the answer to the current exercise. Each exercise can be answered once.</summary>
    public AnswerResult Submit(string? given)
    {
        if (IsFinished)
        {
            throw new InvalidOperationException("The session is finished.");
        }

        if (IsAnswered)
        {
            throw new InvalidOperationException("The current exercise has already been answered.");
        }

        var exercise = _exercises[_index];
        var result = new AnswerResult(exercise, (given ?? string.Empty).Trim(),
            AnswerChecker.IsCorrect(given, exercise.Answer));
        _results.Add(result);
        return result;
    }

    /// <summary>Moves to the next exercise; only allowed after answering the current one.</summary>
    public void Next()
    {
        if (!IsAnswered)
        {
            throw new InvalidOperationException("Answer the current exercise first.");
        }

        _index++;
    }

    /// <summary>A new session with only the exercises that were answered wrongly.</summary>
    public TrainingSession CreateRetryOfMistakes()
    {
        var mistakes = Mistakes;
        if (mistakes.Count == 0)
        {
            throw new InvalidOperationException("There are no mistakes to retry.");
        }

        return new TrainingSession(mistakes.Select(m => m.Exercise));
    }
}
