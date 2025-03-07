using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MY.QuickAPI.Attributes;
using MY.QuickAPI.Database.Services.Core;

namespace MY.QuickAPI.Core.Definitions;

[DefinitionOrder(-10)]
internal class TenantProviderDefinition : IDefinition
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