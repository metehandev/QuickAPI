using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace QuickAPI.Core;

/// <summary>
/// Base interface to implement for this library.
/// </summary>
public interface IDefinition
{
    /// <summary>
    /// App usages
    /// </summary>
    /// <param name="app"></param>
    public void Define(WebApplication app);
    
    /// <summary>
    /// DI Registrations
    /// </summary>
    /// <param name="services"></param>
    public void DefineServices(IServiceCollection services);
}