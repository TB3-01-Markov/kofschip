namespace Kofschip.Core.Training;

/// <summary>Draws a random selection without repeats.</summary>
public static class ExercisePicker
{
    /// <summary>
    /// Returns <paramref name="count"/> different items in random order,
    /// or all of them when the pool is smaller.
    /// </summary>
    public static IReadOnlyList<T> Pick<T>(IEnumerable<T> pool, int count, Random random)
    {
        ArgumentNullException.ThrowIfNull(pool);
        ArgumentNullException.ThrowIfNull(random);
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        var items = pool.ToArray();
        random.Shuffle(items);
        return items.Take(count).ToList();
    }
}
