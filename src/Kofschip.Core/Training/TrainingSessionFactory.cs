using Kofschip.Core.Conjugation;

namespace Kofschip.Core.Training;

/// <summary>Builds a session from the learner's settings.</summary>
public sealed class TrainingSessionFactory(ISentenceSource sentences, ConjugationEngine engine, Random? random = null)
{
    private readonly Random _random = random ?? Random.Shared;

    /// <summary>
    /// Returns null when the settings select no exercises (no tense chosen, or no data).
    /// </summary>
    public async Task<TrainingSession?> CreateAsync(
        TrainingSettings settings, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);

        if (!settings.CanStart)
        {
            return null;
        }

        var pool = await sentences.GetByTensesAsync(settings.Tenses, cancellationToken);
        var exercises = ExercisePicker.Pick(pool, settings.Length, _random)
            .Select(s => new Exercise(s, engine.Conjugate(s.Verb, s.Tense, s.Person, s.Inversion)))
            .ToList();

        return exercises.Count == 0 ? null : new TrainingSession(exercises);
    }
}
