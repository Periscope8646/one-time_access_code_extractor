using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using one_time_access_code_extractor.ConstValues;
using one_time_access_code_extractor.Utils;

namespace one_time_access_code_extractor.Middleware;

public class UserIdClaimMiddleware
{
    private readonly RequestDelegate _next;

    public UserIdClaimMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var requiresAuth = endpoint?.Metadata?.GetMetadata<AuthorizeAttribute>() != null;

        if (requiresAuth)
        {
            var userId = ClaimHelper.GetUserId(context.User);
            if (string.IsNullOrEmpty(userId))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = ConstMessages.UserIdClaimNotFound });
                return;
            }
        }

        await _next(context);
    }
}