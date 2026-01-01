using Google.Apis.Gmail.v1.Data;
using Microsoft.Extensions.Logging;
using one_time_access_code_extractor.Policies;
using one_time_access_code_extractor.Services.Auth.GoogleAuth;
using one_time_access_code_extractor.Services.Base;

namespace one_time_access_code_extractor.Services.Gmail;

public interface IGmailService
{
    Task<Profile> GetProfile(string requestingApplicationUserId);
}

public class GmailService(
    IGoogleAuthService googleAuthService,
    ILogger<GmailService> logger,
    IAuthorizedGmailAsyncPolicy authorizedGmailAsyncPolicy)
    : GmailAuthorizedServiceBase("Gmail", googleAuthService, logger, authorizedGmailAsyncPolicy), IGmailService
{
    public async Task<Profile> GetProfile(string requestingApplicationUserId)
    {
        return await ExecuteWithRetryAsync(async () =>
        {
            var gmailService = await InitializeGmailServiceAsync(requestingApplicationUserId);
            var profileRequest = gmailService.Users.GetProfile("me");
            var profileResponse = await profileRequest.ExecuteAsync();
            return profileResponse;
        });
    }
}