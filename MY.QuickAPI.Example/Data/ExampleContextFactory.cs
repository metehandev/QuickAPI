using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MY.QuickAPI.Example.Data.Providers;

namespace MY.QuickAPI.Example.Data;

public class ExampleContextFactory : IDesignTimeDbContextFactory<ExampleContext>
{
    public ExampleContext CreateDbContext(string[] args)
    {
        const string connectionString =
            "Server=192.168.2.240;Initial Catalog=QuickApiExample;Persist Security Info=False;User ID=sa;Password=My6031*+;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";
        var builder = new DbContextOptionsBuilder<ExampleContext>();
        builder.UseSqlServer(connectionString);

        var tenantProvider = new FakeTenantProvider();

        return new ExampleContext(builder.Options, tenantProvider);
    }
}