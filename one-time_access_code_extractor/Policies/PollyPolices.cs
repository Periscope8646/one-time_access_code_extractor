using System.Net;
using Google;
using Microsoft.Extensions.DependencyInjection;
using one_time_access_code_extractor.Services.Auth.GoogleAuth;
using Polly;

namespace one_time_access_code_extractor.Policies;

public static class PollyPolicies
{
    public static IAsyncPolicy GetAuthorizedGmailRetryPolicy(IServiceProvider sp)
    {
        return Policy
            .Handle<UnauthorizedAccessException>()
            .Or<GoogleApiException>(ex => ex.HttpStatusCode == HttpStatusCode.Unauthorized)
            .RetryAsync(1, async (exception, retryCount, context) =>
            {
                using var scope = sp.CreateScope();
                var googleAuthService = scope.ServiceProvider.GetRequiredService<IGoogleAuthService>();
                var refreshed = await googleAuthService.RefreshTokenAsync();
                if (!refreshed)
                    throw new UnauthorizedAccessException("Gmail (Google) refresh token invalid. User must re-login.");
            });
    }

}