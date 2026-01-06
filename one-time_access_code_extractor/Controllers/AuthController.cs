using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using one_time_access_code_extractor.Services.Auth;

namespace one_time_access_code_extractor.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _authService;

    public AuthController(ILogger<AuthController> logger, IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    /*[AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        try
        {
            var result =  await _authService.RegisterAsync(request);
            if (!result.Success) return Unauthorized(result);
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during registration for user {UserName}", request.Email);
            return StatusCode(500, new { error = "Error occured while processing your request" });
        }
    }*/

    [AllowAnonymous]
    [HttpPost("login-jwt")]
    public async Task<IActionResult> LoginJwt([FromBody] LoginDto request)
    {
        try
        {
            var result = await _authService.LoginJwtAsync(request);
            if (!result.Success) return Unauthorized(result);
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during JWT login for user {UserNameOrEmail}", request.Email);
            return StatusCode(500, new { error = "Error occured while processing your request" });
        }
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var userId =User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        return Ok(new { userId, userName, email });
    }
}