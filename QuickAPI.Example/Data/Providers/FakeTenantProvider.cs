using QuickAPI.Database.Services.Core;

namespace QuickAPI.Example.Data.Providers;

public class FakeTenantProvider : ITenantProvider
{
    public Guid? GetCurrentTenantId()
    {
        return null;
    }

    public string? GetCurrentUser()
    {
        return null;
    }
}