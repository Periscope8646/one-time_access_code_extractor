using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using one_time_access_code_extractor.DTOs;
using one_time_access_code_extractor.Services;

namespace one_time_access_code_extractor.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class DiscordWhitelistController : ControllerBase
{
    private readonly IDiscordWhitelistService _discordWhitelistService;

    public DiscordWhitelistController(IDiscordWhitelistService discordWhitelistService)
    {
        _discordWhitelistService = discordWhitelistService;
    }

    [HttpPost("save_or_update_user")]
    public async Task<IActionResult> SaveOrUpdateUserAsync(DiscordWhitelistDto discordWhitelistDto)
    {
        await _discordWhitelistService.SaveOrUpdateAsync(discordWhitelistDto);
        return Ok();
    }
}