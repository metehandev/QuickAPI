using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using QuickAPI.Database.Services.Core;

namespace QuickAPI.Core;

public class TenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? GetCurrentTenantId()
    {
        var tenantId = _httpContextAccessor.HttpContext?.User.FindFirst("TenantId")?.Value;
        return tenantId != null ? Guid.Parse(tenantId) : null;
    }

    public string? GetCurrentUser()
    {
        var user = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value;
        return user;
    }
} 