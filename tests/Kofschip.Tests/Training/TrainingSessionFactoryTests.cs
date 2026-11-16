using Kofschip.Core.Conjugation;
using Kofschip.Core.Model;
using Kofschip.Core.Training;

namespace Kofschip.Tests.Training;

public class TrainingSessionFactoryTests
{
    private sealed class FakeSentenceSource(IEnumerable<Sentence> sentences) : ISentenceSource
    {
        public IReadOnlyCollection<Tense>? RequestedTenses { get; private set; }

        public Task<IReadOnlyList<Sentence>> GetByTensesAsync(
            IReadOnlyCollection<Tense> tenses, CancellationToken cancellationToken = default)
        {
            RequestedTenses = tenses;
            IReadOnlyList<Sentence> result = sentences.Where(s => tenses.Contains(s.Tense)).ToList();
            return Task.FromResult(result);
        }
    }

    private static List<Sentence> Sentences(int perTense)
    {
        var result = new List<Sentence>();
        for (var i = 0; i < perTense; i++)
        {
            result.Add(TestData.Sentence("werken", Tense.Presens, $"Ik ___ hier. ({i})"));
            result.Add(TestData.Sentence("werken", Tense.Imperfectum, $"Ik ___ daar. ({i})"));
            result.Add(TestData.Sentence("werken", Tense.Perfectum, $"Ik heb ___. ({i})"));
        }

        return result;
    }

    private static TrainingSessionFactory Factory(ISentenceSource source) =>
        new(source, new ConjugationEngine(), new Random(3));

    [Fact]
    public async Task Creates_a_session_of_the_requested_length()
    {
        var source = new FakeSentenceSource(Sentences(40));

        var session = await Factory(source).CreateAsync(new TrainingSettings([Tense.Imperfectum], 20));

        Assert.NotNull(session);
        Assert.Equal(20, session.Total);
        Assert.Equal(new[] { Tense.Imperfectum }, source.RequestedTenses);
    }

    [Fact]
    public async Task Exercises_carry_the_conjugated_answer()
    {
        var source = new FakeSentenceSource(Sentences(1));

        var session = await Factory(source).CreateAsync(new TrainingSettings([Tense.Perfectum]));

        Assert.NotNull(session);
        Assert.Equal("gewerkt", session.Current.Answer);
        Assert.NotEmpty(session.Current.Form.Steps);
    }

    [Fact]
    public async Task Session_is_shorter_when_there_are_too_few_sentences()
    {
        var source = new FakeSentenceSource(Sentences(4));

        var session = await Factory(source).CreateAsync(new TrainingSettings([Tense.Presens], 30));

        Assert.NotNull(session);
        Assert.Equal(4, session.Total);
    }

    [Fact]
    public async Task Returns_null_without_tenses() =>
        Assert.Null(await Factory(new FakeSentenceSource(Sentences(5))).CreateAsync(new TrainingSettings([])));

    [Fact]
    public async Task Returns_null_without_sentences() =>
        Assert.Null(await Factory(new FakeSentenceSource([])).CreateAsync(TrainingSettings.Default));
}
