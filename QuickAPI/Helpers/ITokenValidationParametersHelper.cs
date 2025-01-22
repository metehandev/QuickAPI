using Microsoft.IdentityModel.Tokens;

namespace QuickAPI.Helpers;

public interface ITokenValidationParametersHelper : IHelper
{
    public TokenValidationParameters GetValidationParameters();
}