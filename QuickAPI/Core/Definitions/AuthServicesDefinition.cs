using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using QuickAPI.Helpers;

namespace QuickAPI.Core.Definitions;

public class AuthServicesDefinition(
    ITokenValidationParametersHelper tokenValidationParametersHelper) : IDefinition
{
    // private readonly TokenServiceSettings _settings = options.Value;

    public void Define(WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        //
        app.Use(async (context, next) =>
        {
            var middleware = context.RequestServices.GetService<IAuthorizationMiddleware>();
            if (middleware is null)
            {
                await next.Invoke(context);
                return;
            }

            await middleware.InvokeAsync(context, next);
        });
    }

    public void DefineServices(IServiceCollection services)
    {
        services.AddAuthorization();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = tokenValidationParametersHelper.GetValidationParameters();
            opt.Validate();
        });
    }
}