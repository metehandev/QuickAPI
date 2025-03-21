using Microsoft.EntityFrameworkCore;
using MY.QuickAPI.Attributes;
using MY.QuickAPI.Core;
using MY.QuickAPI.Database.Data;
using MY.QuickAPI.Example.Data;

namespace MY.QuickAPI.Example.Definitions;

[DefinitionOrder(0)]
public class DbContextDefinition : IDefinition
{
    public void Define(WebApplication app)
    {
    }

    public void DefineServices(IServiceCollection services)
    {
        services.AddDbContext<BaseContext, ExampleContext>(options =>
        {
            const string connString =
                "Server=192.168.2.240;Initial Catalog=QuickApiExample;Persist Security Info=False;User ID=sa;Password=My6031*+;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";
            options.UseSqlServer(connString);
        });
    }
}