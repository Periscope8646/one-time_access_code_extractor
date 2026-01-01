using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using one_time_access_code_extractor.DTOs.Auth;
using one_time_access_code_extractor.Entities;
using one_time_access_code_extractor.Repositories.Auth;

namespace one_time_access_code_extractor.Services.Auth;

public interface IAuthService
{
    Task<AuthenticationResult> RegisterAsync(RegisterDto request);
    Task<AuthenticationResult> LoginJwtAsync(LoginDto request);
}

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repo;
    private readonly IConfiguration _config;

    public AuthService(IAuthRepository repo, IConfiguration config)
    {
        _repo = repo;
        _config = config;
    }

    public async Task<AuthenticationResult> RegisterAsync(RegisterDto request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Username,
            Email = request.Email,
            EmailConfirmed = true
        };
        var result = await _repo.CreateUserAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return new AuthenticationResult
            {
                Success = false,
                Message = $"User registration failed: {errors}",
                Token = string.Empty,
                ExpiresAt = null
            };
        }
        return new AuthenticationResult
        {
            Success = true,
            Message = "User registered successfully",
        };
    }

    public async Task<AuthenticationResult> LoginJwtAsync(LoginDto request)
    {
        var user = await _repo.FindByNameOrEmailAsync(request.Email);
        if (user == null || !await _repo.CheckPasswordAsync(user, request.Password))
        {
            return new AuthenticationResult
            {
                Success = false,
                Message = "Invalid username or password",
                Token = string.Empty,
                ExpiresAt = null
            };
        }
        var jwt = GenerateJwt(user);
        return new AuthenticationResult
        {
            Success = true,
            Message = "Login successful",
            Token = jwt.AccessToken,
            ExpiresAt = jwt.ExpiresAtUtc
        };
    }

    private JtwResult GenerateJwt(ApplicationUser user)
    {
        var issuer = _config["Jwt:Issuer"] ?? "JunieWebAPI";
        var audience = _config["Jwt:Audience"] ?? issuer;
        var key = _config["Jwt:Key"] ?? "SuperSecretDevelopmentKey123!ChangeMe";
        var expiresMinutes = int.TryParse(_config["Jwt:ExpiresMinutes"], out var m) ? m : 60;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty)
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(expiresMinutes);

        var jwt = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: signingCredentials
        );

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        return new JtwResult(token, "Bearer", expires);
    }
}
