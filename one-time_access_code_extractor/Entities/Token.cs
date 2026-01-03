namespace one_time_access_code_extractor.Entities;

public abstract class Token
{
    public int Id { get; set; }
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}