using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using one_time_access_code_extractor.Data;
using one_time_access_code_extractor.Entities;

namespace one_time_access_code_extractor.Repositories.Base;

public interface ITokenBaseRepository<T> where T : Token
{
    Task SaveTokenAsync(string accessToken, string? refreshToken, int? expiresIn);
    Task<string?> GetRefreshTokenAsync();
    Task<string> GetAccessTokenAsync();
}

public class TokenBaseRepository<T> : ITokenBaseRepository<T> where T : Token
{
    private readonly string _tokenProvider;
    private readonly ILogger<TokenBaseRepository<T>> _logger;
    private readonly ApplicationDbContext _db;
    private readonly DbSet<T> _dbSet;

    public TokenBaseRepository(string tokenProvider, ILogger<TokenBaseRepository<T>> logger,
        ApplicationDbContext db, DbSet<T> dbSet)
    {
        _tokenProvider = tokenProvider;
        _logger = logger;
        _db = db;
        _dbSet = dbSet;
    }

    public async Task SaveTokenAsync(string accessToken, string? refreshToken, int? expiresIn)
    {
        var existingTokens = await _dbSet.ToListAsync();
        _dbSet.RemoveRange(existingTokens);

        var newTokens = Activator.CreateInstance<T>();

        newTokens.AccessToken = accessToken;
        newTokens.RefreshToken = refreshToken!;
        newTokens.ExpiresAt = DateTime.UtcNow.AddSeconds((double)expiresIn!);

        await _dbSet.AddAsync(newTokens);

        _logger.LogInformation("Created new {TokenProvider} token", _tokenProvider);


        await _db.SaveChangesAsync();
    }

    public async Task<string?> GetRefreshTokenAsync()
    {
        return await _dbSet
            .Select(x => x.RefreshToken)
            .FirstOrDefaultAsync();
    }

    public async Task<string> GetAccessTokenAsync()
    {
        return await _dbSet
            .Select(x => x.AccessToken)
            .FirstOrDefaultAsync() ?? throw new ArgumentException("No access token found");
    }
}