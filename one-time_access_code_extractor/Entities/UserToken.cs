namespace one_time_access_code_extractor.Entities;

public abstract class UserToken
{
    public int Id { get; set; }
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public virtual required ApplicationUser User { get; set; }
}