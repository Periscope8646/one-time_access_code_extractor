using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using one_time_access_code_extractor.DTOs.Gmail;
using one_time_access_code_extractor.Policies;
using one_time_access_code_extractor.Services.Auth.GoogleAuth;
using one_time_access_code_extractor.Services.Base;
using one_time_access_code_extractor.Utils;

namespace one_time_access_code_extractor.Services.Gmail;

public interface IDiscordDisneyPlusGmailService
{
    Task<DisneyPlusResponseDto> GetAccessCode();
}

public class DiscordDisneyPlusGmailService : GmailAuthorizedServiceBase, IDiscordDisneyPlusGmailService
{
    private readonly ILogger<DiscordDisneyPlusGmailService> _logger;
    private readonly IGmailServiceHelper _gmailServiceHelper;

    public DiscordDisneyPlusGmailService(
        IConfiguration configuration,
        IGoogleAuthService googleAuthService,
        ILogger<DiscordDisneyPlusGmailService> logger,
        IAuthorizedGmailAsyncPolicy authorizedGmailAsyncPolicy, IGmailServiceHelper gmailServiceHelper)
        : base("Gmail", googleAuthService, logger, authorizedGmailAsyncPolicy)
    {
        _logger = logger;
        _gmailServiceHelper = gmailServiceHelper;
    }

    public Task<DisneyPlusResponseDto> GetAccessCode()
    {
        throw new NotImplementedException();
    }
}