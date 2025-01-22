namespace QuickAPI.Shared.Dtos;

public record TokenDataDto(string UserName, Guid TenantId, string Role, Guid UserId, string Email);
