using Kofschip.Core.Training;

namespace Kofschip.Tests.Training;

public class ExercisePickerTests
{
    private static readonly int[] Pool = Enumerable.Range(1, 50).ToArray();

    [Fact]
    public void Picks_the_requested_number_without_repeats()
    {
        var picked = ExercisePicker.Pick(Pool, 20, new Random(1));

        Assert.Equal(20, picked.Count);
        Assert.Equal(20, picked.Distinct().Count());
        Assert.All(picked, item => Assert.Contains(item, Pool));
    }

    [Fact]
    public void Returns_everything_when_the_pool_is_small()
    {
        var picked = ExercisePicker.Pick(new[] { 1, 2, 3 }, 10, new Random(1));

        Assert.Equal(new[] { 1, 2, 3 }, picked.Order());
    }

    [Fact]
    public void Same_seed_gives_the_same_selection() =>
        Assert.Equal(
            ExercisePicker.Pick(Pool, 10, new Random(42)),
            ExercisePicker.Pick(Pool, 10, new Random(42)));

    [Fact]
    public void Different_seeds_give_a_different_order() =>
        Assert.NotEqual(
            ExercisePicker.Pick(Pool, 10, new Random(1)),
            ExercisePicker.Pick(Pool, 10, new Random(2)));

    [Fact]
    public void Does_not_change_the_pool()
    {
        var pool = Pool.ToArray();

        ExercisePicker.Pick(pool, 10, new Random(7));

        Assert.Equal(Pool, pool);
    }

    [Fact]
    public void Zero_gives_an_empty_selection() =>
        Assert.Empty(ExercisePicker.Pick(Pool, 0, new Random(1)));
}
