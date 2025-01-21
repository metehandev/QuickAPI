using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace QuickAPI.Core.Definitions;

public class AuthServicesDefinition : IDefinition
{
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

    private TokenValidationParameters GetValidationParameters(IConfiguration configuration)
    {
        if (configuration is null)
        {
            throw new Exception("Configuration is not loaded correctly!");
        }

        var key = configuration.GetValue("TokenService:Key",
            "C2C963B5F00ADC4D1D52A79B1762B808AC9120AEC2598122F10ABD227302D328")!;
        var issuer = configuration.GetValue("TokenService:Issuer", "www.merkez.com.tr");
        var audience = configuration.GetValue("TokenService:Audience", "https://localhost:5001");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = securityKey
        };
    }

    public void DefineServices(IServiceCollection services)
    {
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

        services.AddAuthorization();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = GetValidationParameters(configuration);
            opt.Validate();
        });
    }
}