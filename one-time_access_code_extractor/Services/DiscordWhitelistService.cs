using one_time_access_code_extractor.DTOs;
using one_time_access_code_extractor.Repositories;

namespace one_time_access_code_extractor.Services;

public interface IDiscordWhitelistService
{
    Task SaveOrUpdateAsync(DiscordWhitelistDto discordWhitelistDto);
    Task<bool> IsUserWhitelistedAsync(ulong discordUserId, string platformName);
}

public class DiscordWhitelistService : IDiscordWhitelistService
{
    private readonly IDiscordWhitelistRepository _discordWhitelistRepository;

    public DiscordWhitelistService(IDiscordWhitelistRepository discordWhitelistRepository)
    {
        _discordWhitelistRepository = discordWhitelistRepository;
    }

    public async Task SaveOrUpdateAsync(DiscordWhitelistDto discordWhitelistDto)
    {
        await _discordWhitelistRepository.SaveOrUpdateAsync(discordWhitelistDto);
    }

    public Task<bool> IsUserWhitelistedAsync(ulong discordUserId, string platformName)
    {
        return _discordWhitelistRepository.IsUserWhitelistedAsync(discordUserId, platformName);
    }
}