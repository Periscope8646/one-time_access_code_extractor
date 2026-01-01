using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using one_time_access_code_extractor.Data;
using one_time_access_code_extractor.Entities;

namespace one_time_access_code_extractor.Repositories.Base;

public interface IUserTokenBaseRepository<T> where T : UserToken
{
    Task SaveOrUpdateUserTokensAsync(string userId, string accessToken, string? refreshToken, int? expiresIn);
    Task<string?> GetRefreshTokenAsync(string userId);
    Task<string> GetAccessTokenAsync(string userId);
}

public class UserTokenBaseRepository<T> : IUserTokenBaseRepository<T> where T : UserToken
{
    private readonly string _tokenProvider;
    private readonly ILogger<UserTokenBaseRepository<T>> _logger;
    private readonly ApplicationDbContext _db;
    private readonly DbSet<T> _dbSet;

    public UserTokenBaseRepository(string tokenProvider, ILogger<UserTokenBaseRepository<T>> logger, ApplicationDbContext db, DbSet<T> dbSet)
    {
        _tokenProvider = tokenProvider;
        _logger = logger;
        _db = db;
        _dbSet = dbSet;
    }

    public async Task SaveOrUpdateUserTokensAsync(string userId, string accessToken, string? refreshToken, int? expiresIn)
    {
        var existingTokens = await _dbSet.FirstOrDefaultAsync(x => x.User.Id == userId);

        if (existingTokens != null)
        {
            existingTokens.AccessToken = accessToken;
            existingTokens.RefreshToken = refreshToken ?? existingTokens.RefreshToken;
            existingTokens.ExpiresAt = expiresIn.HasValue
                ? DateTime.UtcNow.AddSeconds(expiresIn.Value)
                : existingTokens.ExpiresAt;
            _dbSet.Update(existingTokens);

            _logger.LogInformation("Updated existing {TokenProvider} tokens for user {UserId}", _tokenProvider, userId);
        }
        else
        {
            var newTokens = Activator.CreateInstance<T>();

            newTokens.User = await _db.Users.FirstAsync(x => x.Id == userId);
            newTokens.AccessToken = accessToken;
            newTokens.RefreshToken = refreshToken!;
            newTokens.ExpiresAt = DateTime.UtcNow.AddSeconds((double)expiresIn!);

            await _dbSet.AddAsync(newTokens);

            _logger.LogInformation("Created new {TokenProvider} tokens for user {UserId}", _tokenProvider, userId);
        }

        await _db.SaveChangesAsync();
    }

    public async Task<string?> GetRefreshTokenAsync(string userId)
    {
        return await _dbSet
            .Where(x => x.User.Id == userId)
            .Select(x => x.RefreshToken)
            .FirstOrDefaultAsync();
    }

    public async Task<string> GetAccessTokenAsync(string userId)
    {
        return await _dbSet
            .Where(x => x.User.Id == userId)
            .Select(x => x.AccessToken)
            .FirstOrDefaultAsync() ?? throw new ArgumentException("No access token found for user", nameof(userId));
    }
}