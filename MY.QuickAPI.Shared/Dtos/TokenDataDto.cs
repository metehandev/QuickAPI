namespace MY.QuickAPI.Shared.Dtos;

/// <summary>
/// Simple token data record. 
/// </summary>
/// <param name="UserName"></param>
/// <param name="TenantId"></param>
/// <param name="Role"></param>
/// <param name="UserId"></param>
/// <param name="Email"></param>
public record TokenDataDto(string UserName, Guid TenantId, string Role, Guid UserId, string Email);
