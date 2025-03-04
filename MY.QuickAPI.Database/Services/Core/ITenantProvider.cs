namespace MY.QuickAPI.Database.Services.Core;

/// <summary>
/// TenantProvider interface to implement on. Will be injected into BaseContext.
/// </summary>
public interface ITenantProvider
{
    /// <summary>
    /// Implement this for getting the current TenantId for context.
    /// In example project we used HttpContext and got the TenantId from Claims.
    /// </summary>
    /// <returns></returns>
    public Guid? GetCurrentTenantId();
    
    /// <summary>
    /// Implement this for getting the current UserName for context.
    /// In example project we used HttpContext and got the UserName from Claims.
    /// </summary>
    /// <returns></returns>
    public string? GetCurrentUser();
}