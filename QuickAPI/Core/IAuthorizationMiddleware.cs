using Microsoft.AspNetCore.Http;

namespace QuickAPI.Core;

public interface IAuthorizationMiddleware
{
    Task InvokeAsync(HttpContext context, RequestDelegate next);
}