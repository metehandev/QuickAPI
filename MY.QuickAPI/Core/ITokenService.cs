using MY.QuickAPI.Shared.Dtos;

namespace MY.QuickAPI.Core;

/// <summary>
/// Base TokenService interface
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Build Bearer token from TokenDataDto
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    string BuildToken(TokenDataDto user);
}