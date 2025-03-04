using Microsoft.AspNetCore.Http;

namespace MY.QuickAPI.Core;

/// <summary>
/// Overridable middleware to add extra layers on Authorization
/// </summary>
public interface IAuthorizationMiddleware
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    Task InvokeAsync(HttpContext context, RequestDelegate next);
}