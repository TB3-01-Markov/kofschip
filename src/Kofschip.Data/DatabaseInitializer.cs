using Kofschip.Data.Seed;
using Microsoft.EntityFrameworkCore;

namespace Kofschip.Data;

/// <summary>Brings the database up to date when the application starts.</summary>
public static class DatabaseInitializer
{
    /// <summary>
    /// Applies pending migrations and adds seed verbs that are not in the database yet.
    /// Safe to run on every start: existing verbs are left alone.
    /// </summary>
    /// <returns>The number of verbs that were added.</returns>
    public static async Task<int> InitializeAsync(KofschipDbContext db, CancellationToken cancellationToken = default)
    {
        await db.Database.MigrateAsync(cancellationToken);

        var existing = await db.Verbs.Select(v => v.Infinitive).ToListAsync(cancellationToken);
        var known = existing.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missing = SeedReader.ReadVerbs().Where(v => !known.Contains(v.Infinitive)).ToList();

        if (missing.Count > 0)
        {
            db.Verbs.AddRange(missing);
            await db.SaveChangesAsync(cancellationToken);
        }

        return missing.Count;
    }
}
