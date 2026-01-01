using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using one_time_access_code_extractor.ConstValues;
using one_time_access_code_extractor.Repositories.GoogleAuth;

namespace one_time_access_code_extractor.Services.Auth.GoogleAuth;

public interface IGoogleAuthService
{
    Task<string> LoginCallbackAsync(string userId, string code);
    Task<bool> RefreshTokenAsync(string userId);
    Task<string> GetAccessTokenAsync(string userId);
}

public class GoogleAuthService : IGoogleAuthService
{
    private readonly ILogger<GoogleAuthService> _logger;
    private readonly IGoogleUserTokenRepository _googleUserTokenRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IServiceProvider _serviceProvider;

    private readonly string? _tokenUrl;
    private readonly string? _clientId;
    private readonly string? _clientSecret;
    private readonly string? _redirectUri;

    private HttpClient HttpClient => _httpClientFactory.CreateClient(HttpClients.GoogleOAuthClient);

    public GoogleAuthService(
        ILogger<GoogleAuthService> logger,
        IConfiguration config,
        IGoogleUserTokenRepository googleUserTokenRepository,
        IHttpClientFactory httpClientFactory,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _googleUserTokenRepository = googleUserTokenRepository;
        _httpClientFactory = httpClientFactory;
        _serviceProvider = serviceProvider;

        _clientId = config["GoogleAuth:ClientId"] ?? throw new InvalidOperationException("Google Client ID is not configured.");
        _clientSecret = config["GoogleAuth:ClientSecret"] ?? throw new InvalidOperationException("Google Client Secret is not configured.");
        _redirectUri = config["GoogleAuth:RedirectUri"] ?? throw new InvalidOperationException("Google Redirect URI is not configured.");
        _tokenUrl = config["GoogleAuth:TokenUrl"] ?? throw new InvalidOperationException("Google Token URL is not configured.");
    }

    private async Task CreateOrUpdateUserTokens(string userId, string accessToken, string? refreshToken = null, int? expiresIn = null)
    {
        await _googleUserTokenRepository.SaveOrUpdateUserTokensAsync(userId, accessToken, refreshToken, expiresIn);
        _logger.LogInformation("Saved or updated Google tokens for user {UserId}", userId);
    }

    private async Task<string> GetRefreshTokenAsync(string userId)
    {
        var token = await _googleUserTokenRepository.GetRefreshTokenAsync(userId);
        if (token == null)
        {
            _logger.LogWarning("No refresh token found for user {UserId}", userId);
            throw new ArgumentException("No refresh token found for user", nameof(userId));
        }
        return token;
    }

    public async Task<string> LoginCallbackAsync(string userId, string code)
    {
        var tokenResponse = await HttpClient.PostAsync(
            _tokenUrl,
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "code", code },
                { "client_id", _clientId! },
                { "client_secret", _clientSecret! },
                { "redirect_uri", _redirectUri! },
                { "grant_type", "authorization_code" }
            }));

        var payload = await tokenResponse.Content.ReadFromJsonAsync<JsonElement>();
        var accessToken = payload.GetProperty("access_token").GetString()!;
        var refreshToken = payload.GetProperty("refresh_token").GetString()!;
        var expiresIn = payload.GetProperty("expires_in").GetInt32();


        await CreateOrUpdateUserTokens(userId, accessToken, refreshToken, expiresIn);

        return userId;
    }

    public async Task<bool> RefreshTokenAsync(string userId)
    {
        var tokenResponse = await HttpClient.PostAsync(
            _tokenUrl,
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", _clientId! },
                { "client_secret", _clientSecret! },
                { "refresh_token", await GetRefreshTokenAsync(userId) },
                { "grant_type", "refresh_token" }
            }));

        var payload = await tokenResponse.Content.ReadFromJsonAsync<JsonElement>();
        var accessToken = payload.GetProperty("access_token").GetString()!;

        await CreateOrUpdateUserTokens(userId, accessToken);


        return true;
    }

    public Task<string> GetAccessTokenAsync(string userId)
    {
        return _googleUserTokenRepository.GetAccessTokenAsync(userId);
    }
}