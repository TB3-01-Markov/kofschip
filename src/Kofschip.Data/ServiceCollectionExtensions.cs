using Kofschip.Core.Training;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kofschip.Data;

public static class ServiceCollectionExtensions
{
    /// <summary>Registers the SQLite database and the repositories built on it.</summary>
    public static IServiceCollection AddKofschipData(this IServiceCollection services, string connectionString)
    {
        services.AddDbContextFactory<KofschipDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<ISentenceSource, SentenceRepository>();
        return services;
    }

    /// <summary>Migrates and seeds the database; call once at startup.</summary>
    public static async Task InitializeKofschipDatabaseAsync(this IServiceProvider services)
    {
        var factory = services.GetRequiredService<IDbContextFactory<KofschipDbContext>>();
        await using var db = await factory.CreateDbContextAsync();
        await DatabaseInitializer.InitializeAsync(db);
    }
}
