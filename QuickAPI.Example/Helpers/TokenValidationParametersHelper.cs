using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QuickAPI.Attributes;
using QuickAPI.Core;
using QuickAPI.Example.Settings;
using QuickAPI.Helpers;

namespace QuickAPI.Example.Helpers;

[HelperDefinition(DependencyInjectionType = DependencyInjectionType.Singleton)]
public class TokenValidationParametersHelper(IOptions<TokenServiceSettings> options) : ITokenValidationParametersHelper
{

    private readonly TokenServiceSettings _settings = options.Value;
    
    public TokenValidationParameters GetValidationParameters()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
    
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _settings.Issuer,
            ValidAudience = _settings.Audience,
            IssuerSigningKey = securityKey
        };
    }
}