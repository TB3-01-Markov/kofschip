using Kofschip.Core.Model;
using Kofschip.Core.Training;
using Microsoft.EntityFrameworkCore;

namespace Kofschip.Data;

/// <summary>Reads exercises from the database.</summary>
public sealed class SentenceRepository(IDbContextFactory<KofschipDbContext> contextFactory) : ISentenceSource
{
    public async Task<IReadOnlyList<Sentence>> GetByTensesAsync(
        IReadOnlyCollection<Tense> tenses, CancellationToken cancellationToken = default)
    {
        var wanted = tenses.ToArray();

        // A short-lived context per call: a Blazor Server circuit lives much longer than a request.
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.Sentences
            .AsNoTracking()
            .Include(s => s.Verb)
            .Where(s => wanted.Contains(s.Tense))
            .ToListAsync(cancellationToken);
    }
}
