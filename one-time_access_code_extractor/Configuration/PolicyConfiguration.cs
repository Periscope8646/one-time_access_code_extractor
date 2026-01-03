using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using one_time_access_code_extractor.ConstValues;
using one_time_access_code_extractor.Policies;
using one_time_access_code_extractor.Utils;


namespace one_time_access_code_extractor.Configuration;

public static class PolicyConfiguration
{
    public static void ConfigurePolicies(this IServiceCollection services)
    {
        services.AddScoped(CreateAuthorizedGmailAsyncPolicy);
    }

    private static IAuthorizedGmailAsyncPolicy CreateAuthorizedGmailAsyncPolicy(IServiceProvider serviceProvider)
    {
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext?.User.Identity is not { IsAuthenticated: true })
        {
            throw new UnauthorizedAccessException(ConstMessages.UserNotAuthenticated);
        }

        var userIdClaim = ClaimHelper.GetUserId(httpContext.User);
        var policy = userIdClaim == null
            ? throw new UnauthorizedAccessException(ConstMessages.UserIdClaimNotFound)
            : PollyPolicies.GetAuthorizedGmailRetryPolicy(serviceProvider);

        return new AuthorizedGmailPolicy(policy);
    }
}