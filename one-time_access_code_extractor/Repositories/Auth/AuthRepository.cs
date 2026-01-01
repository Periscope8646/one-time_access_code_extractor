using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using one_time_access_code_extractor.Entities;

namespace one_time_access_code_extractor.Repositories.Auth;

public interface IAuthRepository
{
    Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
    Task<ApplicationUser?> FindByNameOrEmailAsync(string userNameOrEmail);
    Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
}


public class AuthRepository : IAuthRepository
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthRepository(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
        => _userManager.CreateAsync(user, password);

    public async Task<ApplicationUser?> FindByNameOrEmailAsync(string userNameOrEmail)
    {
        var byName = await _userManager.FindByNameAsync(userNameOrEmail);
        if (byName != null) return byName;
        return await _userManager.Users.FirstOrDefaultAsync(u => u.Email != null && u.Email == userNameOrEmail);
    }

    public Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        => _userManager.CheckPasswordAsync(user, password);
}
