namespace QuickAPI.Database.Services.Core;

public interface ITenantProvider
{
    public Guid? GetCurrentTenantId();
    public string? GetCurrentUser();
}