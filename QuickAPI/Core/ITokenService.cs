using QuickAPI.Shared.Dtos;

namespace QuickAPI.Core;

public interface ITokenService
{
    string BuildToken(TokenDataDto user);
}