using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using QuickAPI.Database.Core;

namespace QuickAPI.Core;

public abstract class EndPointDefinitionBase : IEndpointDefinition
{
    public virtual CrudOperation CrudOperation { get; set; } = CrudOperation.All;

    public virtual bool RequireAuthorization { get; set; } = true;

    public virtual string CommonRole { get; set; } = "Administrator";

    public virtual Dictionary<HttpMethod, string> MethodRoles { get; set; } = new();
    public virtual Dictionary<HttpMethod, bool> MethodAllowAnonymouses { get; set; } = new();

    private bool IsAllowAnonymous(HttpMethod method)
    {
        return MethodAllowAnonymouses.TryGetValue(method, out var allowAnonymous) && allowAnonymous;
    }


    public abstract void Define(WebApplication app);


    public abstract void DefineServices(IServiceCollection services);


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