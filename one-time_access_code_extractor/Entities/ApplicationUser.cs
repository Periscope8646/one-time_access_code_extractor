using Microsoft.AspNetCore.Identity;

namespace one_time_access_code_extractor.Entities;

public class ApplicationUser : IdentityUser
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<GoogleUserToken>? GoogleUserTokens { get; set; }
}