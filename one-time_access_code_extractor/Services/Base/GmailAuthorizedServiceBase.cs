using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Microsoft.Extensions.Logging;
using one_time_access_code_extractor.Policies;
using one_time_access_code_extractor.Services.Auth.GoogleAuth;

namespace one_time_access_code_extractor.Services.Base;

public class GmailAuthorizedServiceBase(string serviceName, IGoogleAuthService googleAuthService, ILogger logger, IAuthorizedGmailAsyncPolicy authorizedGmailAsyncPolicy)
{
    protected async Task<GmailService> InitializeGmailServiceWithAccessTokenAsync(string requestingApplicationUserId)
    {
        logger.LogInformation("Initializing Gmail {ServiceName} Authorized Service for user {RequestingApplicationUserId}", serviceName, requestingApplicationUserId);

        return new GmailService(new BaseClientService.Initializer
        {
            HttpClientInitializer = GoogleCredential.FromAccessToken(
                await googleAuthService.GetAccessTokenAsync()),
            ApplicationName = "Gmail AccessToken Authorized to one-time_access_code_extractor"
        });
    }

    protected async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> action)
    {
        return await authorizedGmailAsyncPolicy.ExecuteAsync(action);
    }
}