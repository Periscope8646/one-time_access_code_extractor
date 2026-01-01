using System.Security.Claims;

namespace one_time_access_code_extractor.Utils;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
    }
}