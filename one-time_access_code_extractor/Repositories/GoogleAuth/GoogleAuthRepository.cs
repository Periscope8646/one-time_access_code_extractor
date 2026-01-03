using Microsoft.Extensions.Logging;
using one_time_access_code_extractor.Data;
using one_time_access_code_extractor.Entities;
using one_time_access_code_extractor.Repositories.Base;

namespace one_time_access_code_extractor.Repositories.GoogleAuth;

public interface IGoogleTokenRepository : ITokenBaseRepository<GoogleToken>
{
}

public class GoogleTokenTokenRepository : TokenBaseRepository<GoogleToken>, IGoogleTokenRepository
{
    private const string TokenProvider = "Google";
    public GoogleTokenTokenRepository(ILogger<GoogleTokenTokenRepository> logger, ApplicationDbContext db) : base(TokenProvider, logger, db, db.Set<GoogleToken>())
    {
    }
}