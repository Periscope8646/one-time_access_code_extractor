using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using one_time_access_code_extractor.ConstValues;
using one_time_access_code_extractor.Repositories.GoogleAuth;

namespace one_time_access_code_extractor.Services.Auth.GoogleAuth;

public interface IGoogleAuthService
{
    Task LoginCallbackAsync(string code);
    Task<bool> RefreshTokenAsync();
    Task<string> GetAccessTokenAsync();
}

public class GoogleAuthService : IGoogleAuthService
{
    private readonly ILogger<GoogleAuthService> _logger;
    private readonly IGoogleTokenRepository _googleTokenRepository;
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly string? _tokenUrl;
    private readonly string? _clientId;
    private readonly string? _clientSecret;
    private readonly string? _redirectUri;

    private HttpClient HttpClient => _httpClientFactory.CreateClient(HttpClients.GoogleOAuthClient);

    public GoogleAuthService(
        ILogger<GoogleAuthService> logger,
        IConfiguration config,
        IGoogleTokenRepository googleTokenRepository,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _googleTokenRepository = googleTokenRepository;
        _httpClientFactory = httpClientFactory;

        _clientId = config["GoogleAuth:ClientId"] ?? throw new InvalidOperationException("Google Client ID is not configured.");
        _clientSecret = config["GoogleAuth:ClientSecret"] ?? throw new InvalidOperationException("Google Client Secret is not configured.");
        _redirectUri = config["GoogleAuth:RedirectUri"] ?? throw new InvalidOperationException("Google Redirect URI is not configured.");
        _tokenUrl = config["GoogleAuth:TokenUrl"] ?? throw new InvalidOperationException("Google Token URL is not configured.");
    }

    private async Task CreateGoogleToken(string accessToken, string? refreshToken = null, int? accessTokenExpiresIn = null, int? refreshTokenExpiresIn = null)
    {
        await _googleTokenRepository.SaveTokenAsync(accessToken, refreshToken, accessTokenExpiresIn, refreshTokenExpiresIn);
        _logger.LogInformation("Saved or updated Google token");
    }

    private async Task<string> GetRefreshTokenAsync()
    {
        var token = await _googleTokenRepository.GetRefreshTokenAsync();
        if (token == null)
        {
            _logger.LogWarning("No refresh token found");
            throw new ArgumentException("No refresh token found");
        }
        return token;
    }

    public async Task LoginCallbackAsync(string code)
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
        var accessTokenExpireIn = payload.GetProperty("expires_in").GetInt32();
        var refreshTokenExpireIn = payload.GetProperty("refresh_token_expires_in").GetInt32();


        await CreateGoogleToken(accessToken, refreshToken, accessTokenExpireIn, refreshTokenExpireIn);
    }

    public async Task<bool> RefreshTokenAsync()
    {
        var tokenResponse = await HttpClient.PostAsync(
            _tokenUrl,
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", _clientId! },
                { "client_secret", _clientSecret! },
                { "refresh_token", await GetRefreshTokenAsync() },
                { "grant_type", "refresh_token" }
            }));

        var payload = await tokenResponse.Content.ReadFromJsonAsync<JsonElement>();
        var accessToken = payload.GetProperty("access_token").GetString()!;
        var accessTokenExpiresIn = payload.GetProperty("expires_in").GetInt32()!;

        await CreateGoogleToken(accessToken, null, accessTokenExpiresIn, null);


        return true;
    }

    public Task<string> GetAccessTokenAsync()
    {
        return _googleTokenRepository.GetAccessTokenAsync();
    }
}