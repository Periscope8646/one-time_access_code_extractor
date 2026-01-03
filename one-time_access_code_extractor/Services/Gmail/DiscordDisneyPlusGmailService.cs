using Microsoft.Extensions.Logging;
using one_time_access_code_extractor.DTOs.Gmail;
using one_time_access_code_extractor.Policies;
using one_time_access_code_extractor.Services.Auth.GoogleAuth;
using one_time_access_code_extractor.Services.Base;

namespace one_time_access_code_extractor.Services.Gmail;

public interface IDiscordDisneyPlusGmailService
{
    Task<DisneyPlusResponseDto> GetAccessCode();
}

public class DiscordDisneyPlusGmailService : GmailAuthorizedServiceBase, IDiscordDisneyPlusGmailService
{
    private readonly IGmailServiceHelper _gmailServiceHelper;

    public DiscordDisneyPlusGmailService(IGoogleAuthService googleAuthService,
        ILogger<DiscordDisneyPlusGmailService> logger,
        IAuthorizedGmailAsyncPolicy authorizedGmailAsyncPolicy, IGmailServiceHelper gmailServiceHelper)
        : base("Gmail", googleAuthService, logger, authorizedGmailAsyncPolicy)
    {
        _gmailServiceHelper = gmailServiceHelper;
    }

    public async Task<DisneyPlusResponseDto> GetAccessCode()
    {
        return await ExecuteWithRetryAsync(async () =>
        {
            var gmailService = await InitializeGmailServiceWithAccessTokenAsync();

            var messageSummaries = await _gmailServiceHelper.ListRecentMessagesAsync(gmailService);
            if (messageSummaries == null || !messageSummaries.Any()) return new DisneyPlusResponseDto(GmailServiceHelper.SearchHoursLimit);

            var metadataList = await _gmailServiceHelper.FetchMessagesMetadataAsync(gmailService, messageSummaries);

            var targetEmail = metadataList
                .Where(x => x.Subject.Contains(GmailServiceHelper.DisneySubject, StringComparison.InvariantCultureIgnoreCase))
                .OrderByDescending(x => x.ReceivedAt)
                .FirstOrDefault();

            if (targetEmail == null) return new DisneyPlusResponseDto(GmailServiceHelper.SearchHoursLimit);

            return await _gmailServiceHelper.ExtractDisneyCodeFromMessageAsync(gmailService, targetEmail);
        });
    }
}