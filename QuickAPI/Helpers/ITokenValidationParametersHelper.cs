using Microsoft.IdentityModel.Tokens;

namespace QuickAPI.Helpers;

/// <summary>
/// Token validation parameters helper interface
/// </summary>
public interface ITokenValidationParametersHelper : IHelper
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public TokenValidationParameters GetValidationParameters();
}