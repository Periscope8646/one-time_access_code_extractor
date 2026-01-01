namespace one_time_access_code_extractor.DTOs.Auth;

public class AuthenticationResult
{
    public bool Success { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
}