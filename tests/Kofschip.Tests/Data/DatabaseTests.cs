using Kofschip.Core.Model;
using Kofschip.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Kofschip.Tests.Data;

/// <summary>
/// Runs the real migrations and seed against an in-memory SQLite database.
/// The database lives as long as the open connection, so each test gets a fresh one.
/// </summary>
public sealed class DatabaseTests : IDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private readonly DbContextOptions<KofschipDbContext> _options;

    public DatabaseTests()
    {
        _connection.Open();
        _options = new DbContextOptionsBuilder<KofschipDbContext>().UseSqlite(_connection).Options;
    }

    public void Dispose() => _connection.Dispose();

    private KofschipDbContext NewContext() => new(_options);

    [Fact]
    public async Task Initializer_creates_the_schema_and_seeds_all_verbs()
    {
        await using var db = NewContext();

        var added = await DatabaseInitializer.InitializeAsync(db);

        Assert.Equal(100, added);
        Assert.Equal(100, await db.Verbs.CountAsync());
        Assert.Equal(300, await db.Sentences.CountAsync());
    }

    [Fact]
    public async Task Initializer_can_run_again_without_duplicating_data()
    {
        await using (var first = NewContext())
        {
            await DatabaseInitializer.InitializeAsync(first);
        }

        await using var db = NewContext();
        var added = await DatabaseInitializer.InitializeAsync(db);

        Assert.Equal(0, added);
        Assert.Equal(100, await db.Verbs.CountAsync());
    }

    [Fact]
    public async Task Initializer_only_adds_verbs_that_are_missing()
    {
        await using (var first = NewContext())
        {
            await DatabaseInitializer.InitializeAsync(first);
            first.Verbs.Remove(await first.Verbs.SingleAsync(v => v.Infinitive == "werken"));
            await first.SaveChangesAsync();
        }

        await using var db = NewContext();
        var added = await DatabaseInitializer.InitializeAsync(db);

        Assert.Equal(1, added);
        Assert.Equal(3, await db.Sentences.CountAsync(s => s.Verb.Infinitive == "werken"));
    }

    [Fact]
    public async Task Migrations_match_the_model()
    {
        await using var db = NewContext();

        Assert.False(db.Database.HasPendingModelChanges());
    }

    [Fact]
    public async Task Stem_from_the_seed_is_stored()
    {
        await using var db = NewContext();
        await DatabaseInitializer.InitializeAsync(db);

        var verb = await db.Verbs.SingleAsync(v => v.Infinitive == "proberen");

        Assert.Equal("probeer", verb.Stem);
    }

    [Fact]
    public async Task Infinitive_is_unique()
    {
        await using var db = NewContext();
        await DatabaseInitializer.InitializeAsync(db);

        db.Verbs.Add(new Verb { Infinitive = "werken" });

        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public async Task Deleting_a_verb_deletes_its_sentences()
    {
        await using var db = NewContext();
        await DatabaseInitializer.InitializeAsync(db);

        db.Verbs.Remove(await db.Verbs.SingleAsync(v => v.Infinitive == "werken"));
        await db.SaveChangesAsync();

        Assert.Equal(297, await db.Sentences.CountAsync());
    }

    [Fact]
    public async Task Repository_returns_only_the_requested_tenses_with_their_verbs()
    {
        await using (var db = NewContext())
        {
            await DatabaseInitializer.InitializeAsync(db);
        }

        var repository = new SentenceRepository(new TestContextFactory(_options));

        var sentences = await repository.GetByTensesAsync([Tense.Imperfectum, Tense.Perfectum]);

        Assert.Equal(200, sentences.Count);
        Assert.DoesNotContain(sentences, s => s.Tense == Tense.Presens);
        Assert.All(sentences, s => Assert.False(string.IsNullOrEmpty(s.Verb.Infinitive)));
    }

    [Fact]
    public async Task Repository_returns_nothing_without_tenses()
    {
        await using (var db = NewContext())
        {
            await DatabaseInitializer.InitializeAsync(db);
        }

        var repository = new SentenceRepository(new TestContextFactory(_options));

        Assert.Empty(await repository.GetByTensesAsync([]));
    }

    private sealed class TestContextFactory(DbContextOptions<KofschipDbContext> options)
        : IDbContextFactory<KofschipDbContext>
    {
        public KofschipDbContext CreateDbContext() => new(options);
    }
}
