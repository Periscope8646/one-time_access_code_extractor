namespace one_time_access_code_extractor.DTOs.Auth;

public class JtwResult
{
    public string AccessToken { get; set; }
    public string TokenType { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public string? RefreshToken { get; set; }

    public JtwResult(string accessToken, string tokenType, DateTime expiresAtUtc, string? refreshToken = null)
    {
        AccessToken = accessToken;
        TokenType = tokenType;
        ExpiresAtUtc = expiresAtUtc;
        RefreshToken = refreshToken;
    }
}