using Kofschip.Core.Model;

namespace Kofschip.Core.Training;

/// <summary>Where exercises come from. Implemented by the data layer.</summary>
public interface ISentenceSource
{
    /// <summary>All sentences for the given tenses, each with its verb loaded.</summary>
    Task<IReadOnlyList<Sentence>> GetByTensesAsync(
        IReadOnlyCollection<Tense> tenses, CancellationToken cancellationToken = default);
}
