using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace QuickAPI.Core;

public interface IDefinition
{
    public void Define(WebApplication app);
    
    public void DefineServices(IServiceCollection services);
}