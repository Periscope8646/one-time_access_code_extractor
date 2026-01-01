using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using one_time_access_code_extractor.Services.Gmail;
using one_time_access_code_extractor.Utils;

namespace one_time_access_code_extractor.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class GmailController : ControllerBase
{
    private readonly ILogger<GmailController> _logger;
    private readonly IGmailService _gmailService;

    public GmailController(ILogger<GmailController> logger, IGmailService gmailService)
    {
        _logger = logger;
        _gmailService = gmailService;
    }


    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var result = await _gmailService.GetProfile(User.GetUserId());
        return Ok(result);
    }
}