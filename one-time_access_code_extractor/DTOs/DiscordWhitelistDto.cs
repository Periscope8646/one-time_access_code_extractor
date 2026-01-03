namespace one_time_access_code_extractor.DTOs;

public class DiscordWhitelistDto
{
    public ulong DiscordUserId { get; set; }
    public string DiscordUsername { get; set; }
    public string[] Platforms { get; set; } = [];
}