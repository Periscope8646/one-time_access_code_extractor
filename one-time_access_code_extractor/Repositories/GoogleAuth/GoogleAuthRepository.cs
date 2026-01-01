using Microsoft.Extensions.Logging;
using one_time_access_code_extractor.Data;
using one_time_access_code_extractor.Entities;
using one_time_access_code_extractor.Repositories.Base;

namespace one_time_access_code_extractor.Repositories.GoogleAuth;

public interface IGoogleUserTokenRepository : IUserTokenBaseRepository<GoogleUserToken>
{
}

public class GoogleUserTokenUserTokenRepository : UserTokenBaseRepository<GoogleUserToken>, IGoogleUserTokenRepository
{
    private const string TokenProvider = "Google";
    public GoogleUserTokenUserTokenRepository(ILogger<GoogleUserTokenUserTokenRepository> logger, ApplicationDbContext db) : base(TokenProvider, logger, db, db.Set<GoogleUserToken>())
    {
    }
}