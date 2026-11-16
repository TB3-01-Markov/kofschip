using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Kofschip.Data;

/// <summary>
/// Lets "dotnet ef" create the context without starting the web application:
/// dotnet ef migrations add &lt;Name&gt; --project src/Kofschip.Data
/// </summary>
internal sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<KofschipDbContext>
{
    public KofschipDbContext CreateDbContext(string[] args) =>
        new(new DbContextOptionsBuilder<KofschipDbContext>()
            .UseSqlite("Data Source=kofschip.db")
            .Options);
}
