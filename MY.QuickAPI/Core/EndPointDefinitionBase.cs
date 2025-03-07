using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MY.QuickAPI.Database.Core;

namespace MY.QuickAPI.Core;

/// <summary>
/// Base endpoint definition model to override from.
/// </summary>
public abstract class EndPointDefinitionBase : IEndpointDefinition
{
    /// <summary>
    /// Set CrudOperation value on constructor for auto generation of the marked Crud Operations for model
    /// </summary>
    public CrudOperation CrudOperation { get; set; } = CrudOperation.All;

    /// <summary>
    /// Set RequireAuthorization value on constructor for marking this model for Authorization requirements
    /// </summary>
    public bool RequireAuthorization { get; set; } = true;

    /// <summary>
    /// Set CommonRole value on constructor for marking this model's Authorized role for all Crud Operations
    /// </summary>
    public string CommonRole { get; set; } = "Administrator";

    /// <summary>
    /// Add values to this dictionary to change different HTTP Method's Authorized roles
    /// </summary>
    public Dictionary<HttpMethod, string> MethodRoles { get; set; } = new();
    
    /// <summary>
    /// Add values to this dictionary to mark different HTTP Methods as AllowAnonymous
    /// </summary>
    public Dictionary<HttpMethod, bool> MethodAllowAnonymouses { get; set; } = new();

    /// <summary>
    /// Add default Navigation paths for DbQuery includes 
    /// </summary>
    public string[] IncludeFields { get; set; } = [];
    
    private bool IsAllowAnonymous(HttpMethod method)
    {
        return MethodAllowAnonymouses.TryGetValue(method, out var allowAnonymous) && allowAnonymous;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    public abstract void Define(WebApplication app);


    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    public abstract void DefineServices(IServiceCollection services);


    /// <summary>
    /// 
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    protected AuthorizationOptions GenerateAuthorizationOptions(HttpMethod? method = null)
    {
        var authorizedRoles = string.Empty;
        if (!string.IsNullOrEmpty(CommonRole))
        {
            authorizedRoles = CommonRole;
        }

        if (method != null && MethodRoles.TryGetValue(method, out var role) && !string.IsNullOrEmpty(role))
        {
            authorizedRoles = role;
        }

        var authorizeData = new AuthorizationOptions
        {
            Roles = authorizedRoles
        };
        return authorizeData;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="method"></param>
    /// <param name="handlerBuilders"></param>
    protected void BindAuthorizationOptionsForVerb(HttpMethod method, params RouteHandlerBuilder[] handlerBuilders)
    {
        if (!RequireAuthorization || IsAllowAnonymous(method))
        {
            foreach (var handlerBuilder in handlerBuilders)
            {
                handlerBuilder.AllowAnonymous();
            }

            return;
        }

        var options = GenerateAuthorizationOptions(method);
        foreach (var handlerBuilder in handlerBuilders)
        {
            handlerBuilder.RequireAuthorization(options);
        }
    }
}