using Microsoft.AspNetCore.Authorization;

namespace MY.QuickAPI.Core;

/// <summary>
/// Basic Authorization options model to set Policy, Roles and Schemes.
/// </summary>
public class AuthorizationOptions : IAuthorizeData
{
    /// <summary>
    /// Auth policy
    /// </summary>
    public string? Policy { get; set; }
    
    /// <summary>
    /// Authorized Roles
    /// </summary>
    public string? Roles { get; set; }
    
    /// <summary>
    /// Auth Schemes
    /// </summary>
    public string? AuthenticationSchemes { get; set; }
}