using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QuickAPI.Core;
using QuickAPI.Example.Settings;
using QuickAPI.Settings;
using QuickAPI.Shared.Dtos;

namespace QuickAPI.Example.Services;

public class TokenService(IOptions<TokenServiceSettings> options) : ITokenService
{
    private readonly TokenServiceSettings _settings = options.Value;

    public string BuildToken(TokenDataDto user)
    {
        var claims = new[]
        {
            new Claim("TenantId", user.TenantId.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };
        
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key)); 
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
        var tokenDescriptor = new JwtSecurityToken(_settings.Issuer, _settings.Audience, claims,
            expires: DateTime.Now.Add(_settings.ExpiryDuration), signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}