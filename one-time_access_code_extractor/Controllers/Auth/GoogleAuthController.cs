using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using one_time_access_code_extractor.Services.Auth.GoogleAuth;
using one_time_access_code_extractor.Utils;

namespace one_time_access_code_extractor.Controllers.Auth;

[ApiController]
[Route("[controller]")]
public class GoogleAuthController : ControllerBase
{
    private readonly ILogger<GoogleAuthController> _logger;
    private readonly IConfiguration _config;
    private readonly IGoogleAuthService _googleAuthService;

    private readonly string? _scopes;

    public GoogleAuthController(ILogger<GoogleAuthController> logger, IConfiguration config,
        IGoogleAuthService googleAuthService)
    {
        _logger = logger;
        _config = config;
        _googleAuthService = googleAuthService;

        _scopes = _config["GoogleAuth:Scopes"] ?? throw new InvalidOperationException("Google Scopes are not configured.");
    }

    [AllowAnonymous]
    [HttpGet("Login")]
    public async Task<IActionResult> Login()
    {
        try
        {
            var clientId = _config["GoogleAuth:ClientId"] ??
                           throw new InvalidOperationException("Google Client ID is not configured.");
            var redirectUri = _config["GoogleAuth:RedirectUri"] ??
                              throw new InvalidOperationException("Google Redirect URI is not configured.");
            var url =
                $"https://accounts.google.com/o/oauth2/v2/auth?response_type=code&client_id={clientId}&redirect_uri={redirectUri}&scope={_scopes}&access_type=offline&prompt=consent";

            return Redirect(url);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during Google OAuth login redirect");
            return StatusCode(500, new { error = "An error occurred while processing your request." });
        }
    }

    [HttpGet("callback")]
    [Produces("application/json", Type = typeof(string))]
    public async Task<IActionResult> GoogleCallback([FromQuery] string code)
    {
        if (string.IsNullOrEmpty(code)) return BadRequest(new { error = "Authorization code is missing." });

        try
        {
            await _googleAuthService.LoginCallbackAsync(code);
            return Ok("completed");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during Google OAuth callback");
            return StatusCode(500, new { error = "An error occurred while processing your request." });
        }
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        try
        {
            await _googleAuthService.RefreshTokenAsync();

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during Google token refresh");
            return StatusCode(500, new { error = "An error occurred while processing your request." });
        }
    }

}