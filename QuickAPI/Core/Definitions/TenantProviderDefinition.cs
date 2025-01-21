using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using QuickAPI.Database.Services.Core;

namespace QuickAPI.Core.Definitions;

public class TenantProviderDefinition : IDefinition
{
    public void Define(WebApplication app)
    {
        
    }

    public void DefineServices(IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ITenantProvider, TenantProvider>();
    }
}