using System.Net;
using Kofschip.Core.Model;
using Kofschip.Core.Training;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

namespace Kofschip.Tests.Web;

/// <summary>Starts the real application (Program.cs) on a temporary database.</summary>
public sealed class KofschipWebFactory : WebApplicationFactory<Program>
{
    private readonly string _databasePath =
        Path.Combine(Path.GetTempPath(), $"kofschip-test-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder.UseSetting("ConnectionStrings:Kofschip", $"Data Source={_databasePath}");

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        SqliteConnection.ClearAllPools();
        if (File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }
    }
}

public class WebApplicationTests(KofschipWebFactory factory) : IClassFixture<KofschipWebFactory>
{
    [Theory]
    [InlineData("/")]
    [InlineData("/oefenen?tijden=presens&aantal=10")]
    public async Task Pages_are_served(string url)
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Kofschip", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Unknown_page_gives_404()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/bestaat-niet");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Startup_seeds_the_database_and_wires_up_the_services()
    {
        using var scope = factory.Services.CreateScope();
        var sessions = scope.ServiceProvider.GetRequiredService<TrainingSessionFactory>();

        var session = await sessions.CreateAsync(new TrainingSettings([Tense.Perfectum], 20));

        Assert.NotNull(session);
        Assert.Equal(20, session.Total);
    }
}
