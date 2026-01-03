using Microsoft.EntityFrameworkCore;
using one_time_access_code_extractor.Data;
using one_time_access_code_extractor.DTOs;
using one_time_access_code_extractor.Entities;

namespace one_time_access_code_extractor.Repositories;

public interface IDiscordWhitelistRepository
{
    Task SaveOrUpdateAsync(DiscordWhitelistDto discordWhitelistDto);
}

public class DiscordWhitelistRepository : IDiscordWhitelistRepository
{
    private readonly ApplicationDbContext _db;

    public DiscordWhitelistRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task SaveOrUpdateAsync(DiscordWhitelistDto discordWhitelistDto)
    {
        var existingEntry = await _db.DiscordWhitelist
            .FirstOrDefaultAsync(x => x.DiscordUserId == discordWhitelistDto.DiscordUserId);

        if (existingEntry != null)
        {
            existingEntry.DiscordUsername = discordWhitelistDto.DiscordUsername;
            existingEntry.Platforms = discordWhitelistDto.Platforms;
            _db.DiscordWhitelist.Update(existingEntry);
        }
        else
        {
            var newEntry = new DiscordWhitelist
            {
                DiscordUserId = discordWhitelistDto.DiscordUserId,
                DiscordUsername = discordWhitelistDto.DiscordUsername,
                Platforms = discordWhitelistDto.Platforms
            };
            await _db.DiscordWhitelist.AddAsync(newEntry);
        }

        await _db.SaveChangesAsync();
    }
}