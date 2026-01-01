using System.Text;
using System.Text.RegularExpressions;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Requests;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.Configuration;
using one_time_access_code_extractor.DTOs.Gmail;
using one_time_access_code_extractor.Policies;
using one_time_access_code_extractor.Services.Auth.GoogleAuth;
using one_time_access_code_extractor.Services.Base;

namespace one_time_access_code_extractor.Services.Gmail;

public interface IDisneyPlusGmailService
{
    Task<Profile> GetProfile(string requestingApplicationUserId);
    Task<DisneyPlusResponseDto> GetAccessCode(string requestingApplicationUserId);
}

public class DisneyPlusGmailService : GmailAuthorizedServiceBase, IDisneyPlusGmailService
{
    private readonly ILogger<DisneyPlusGmailService> _logger;
    private readonly string _disneyEmail;
    private readonly string _disneySubject;
    private const int SearchHoursLimit = -12;

    public DisneyPlusGmailService(
        IConfiguration configuration,
        IGoogleAuthService googleAuthService,
        ILogger<DisneyPlusGmailService> logger,
        IAuthorizedGmailAsyncPolicy authorizedGmailAsyncPolicy)
        : base("Gmail", googleAuthService, logger, authorizedGmailAsyncPolicy)
    {
        _logger = logger;

        var disneySection = configuration.GetSection("ProviderEmails")
            .GetChildren()
            .FirstOrDefault(s => s.GetValue<string>("Provider") == "DisneyPlus");

        _disneyEmail = disneySection?.GetValue<string>("Email")
                       ?? throw new InvalidConfigurationException("DisneyPlus email not found in configuration.");

        _disneySubject = disneySection?.GetValue<string>("Subject")
                         ?? throw new InvalidConfigurationException(
                             "DisneyPlus subject filter not found in configuration.");
    }

    public async Task<Profile> GetProfile(string requestingApplicationUserId)
    {
        return await ExecuteWithRetryAsync(async () =>
        {
            var gmailService = await InitializeGmailServiceAsync(requestingApplicationUserId);
            return await gmailService.Users.GetProfile("me").ExecuteAsync();
        });
    }

    public async Task<DisneyPlusResponseDto> GetAccessCode(string requestingApplicationUserId)
    {
        return await ExecuteWithRetryAsync(async () =>
        {
            var gmailService = await InitializeGmailServiceAsync(requestingApplicationUserId);

            var messageSummaries = await ListRecentMessagesAsync(gmailService);
            if (messageSummaries == null || !messageSummaries.Any()) return new DisneyPlusResponseDto(SearchHoursLimit);

            var metadataList = await FetchMessagesMetadataAsync(gmailService, messageSummaries);

            var targetEmail = metadataList
                .Where(x => x.Subject.Contains(_disneySubject))
                .OrderByDescending(x => x.ReceivedAt)
                .FirstOrDefault();

            if (targetEmail == null) return new DisneyPlusResponseDto(SearchHoursLimit);

            return await ExtractCodeFromMessageAsync(gmailService, targetEmail);
        });
    }

    private async Task<IList<Message>?> ListRecentMessagesAsync(Google.Apis.Gmail.v1.GmailService service)
    {
        var request = service.Users.Messages.List("me");
        var after = DateTimeOffset.UtcNow.AddHours(SearchHoursLimit).ToUnixTimeSeconds();

        request.Q = $"from:{_disneyEmail} after:{after}";
        var response = await request.ExecuteAsync();

        return response.Messages;
    }

    private async Task<List<EmailMetaData>> FetchMessagesMetadataAsync(GmailService service,
        IList<Message> messages)
    {
        var emailResults = new List<EmailMetaData>();
        var batch = new BatchRequest(service);

        foreach (var msg in messages)
        {
            var getRequest = service.Users.Messages.Get("me", msg.Id);
            getRequest.Format = UsersResource.MessagesResource.GetRequest.FormatEnum.Metadata;

            batch.Queue<Message>(getRequest, (content, error, i, message) =>
            {
                if (error == null)
                {
                    var date = content.InternalDate.HasValue
                        ? DateTimeOffset.FromUnixTimeMilliseconds(content.InternalDate.Value).UtcDateTime
                        : DateTime.MinValue;

                    var subject = content.Payload.Headers.FirstOrDefault(h => h.Name == "Subject")?.Value ??
                                  "NO SUBJECT";
                    emailResults.Add(new EmailMetaData(content.Id, subject, date));
                }
            });
        }

        await batch.ExecuteAsync();
        return emailResults;
    }

    private async Task<DisneyPlusResponseDto> ExtractCodeFromMessageAsync(Google.Apis.Gmail.v1.GmailService service, EmailMetaData emailMetaData)
    {
        var fullMessage = await service.Users.Messages.Get("me", emailMetaData.Id).ExecuteAsync();
        var body = GetDecodedBody(fullMessage.Payload);

        if (string.IsNullOrEmpty(body)) return new DisneyPlusResponseDto(SearchHoursLimit);

        var regex = new Regex(@"^\s*\d{6}\s*$", RegexOptions.Multiline);
        var match = regex.Match(body);

        if (match.Success)
        {
            _logger.LogInformation("Access code found in email {EmailId}", emailMetaData.Id);
            return new DisneyPlusResponseDto(emailMetaData.ReceivedAt, match.Value.Trim(), "Code found");
        }

        _logger.LogWarning("Access code pattern not matched in email {EmailId}", emailMetaData.Id);
        return new DisneyPlusResponseDto(SearchHoursLimit);
    }

    private static string? GetDecodedBody(MessagePart payload)
    {
        var encodedData = payload.Body.Data;

        if (string.IsNullOrEmpty(encodedData) && payload.Parts != null)
        {
            encodedData = payload.Parts.FirstOrDefault(p => p.MimeType == "text/html")?.Body.Data
                          ?? payload.Parts.FirstOrDefault(p => p.MimeType == "text/plain")?.Body.Data;
        }

        return string.IsNullOrEmpty(encodedData) ? null : DecodeBase64Url(encodedData);
    }

    private static string DecodeBase64Url(string base64Url)
    {
        var base64 = base64Url.Replace('-', '+').Replace('_', '/');
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
    }
}