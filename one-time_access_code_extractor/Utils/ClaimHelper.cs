using System.Security.Claims;

namespace one_time_access_code_extractor.Utils;

public static class ClaimHelper
{
    public static string? GetUserId(ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return string.IsNullOrEmpty(userId) ?
            null : userId;
    }
}