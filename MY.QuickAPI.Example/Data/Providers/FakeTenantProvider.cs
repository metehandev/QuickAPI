using MY.QuickAPI.Database.Services.Core;

namespace MY.QuickAPI.Example.Data.Providers;

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