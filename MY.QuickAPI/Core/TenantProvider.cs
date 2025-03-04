using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MY.QuickAPI.Database.Services.Core;

namespace MY.QuickAPI.Core;

/// <summary>
/// Simple ITenantProvider implementation that uses HttpContext and Claims 
/// </summary>
public class TenantProvider(IHttpContextAccessor httpContextAccessor) : ITenantProvider
{
    /// <summary>
    /// Implement this for getting the current TenantId for context.
    /// In example project we used HttpContext and got the TenantId from Claims.
    /// </summary>
    /// <returns></returns>
    public Guid? GetCurrentTenantId()
    {
        var tenantId = httpContextAccessor.HttpContext?.User.FindFirst("TenantId")?.Value;
        return tenantId != null ? Guid.Parse(tenantId) : null;
    }

    /// <summary>
    /// Implement this for getting the current UserName for context.
    /// In example project we used HttpContext and got the UserName from Claims.
    /// </summary>
    /// <returns></returns>
    public string? GetCurrentUser()
    {
        var user = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value;
        return user;
    }
} 