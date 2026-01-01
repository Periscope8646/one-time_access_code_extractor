using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using one_time_access_code_extractor.Services.Gmail;
using one_time_access_code_extractor.Utils;

namespace one_time_access_code_extractor.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class GmailController : ControllerBase
{
    private readonly IDisneyPlusGmailService _disneyPlusGmailService;

    public GmailController(IDisneyPlusGmailService disneyPlusGmailService)
    {
        _disneyPlusGmailService = disneyPlusGmailService;
    }


    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var result = await _disneyPlusGmailService.GetProfile(User.GetUserId());
        return Ok(result);
    }

    [HttpGet("disneyPlus-access_code")]
    public async Task<IActionResult> GetDisneyPlusAccessCode()
    {
        var result = await _disneyPlusGmailService.GetAccessCode(User.GetUserId());
        return Ok(result);
    }
}