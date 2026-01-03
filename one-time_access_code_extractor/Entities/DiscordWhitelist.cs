namespace one_time_access_code_extractor.Entities;

public class DiscordWhitelist
{
    public int Id { get; set; }
    public ulong DiscordUserId { get; set; }
    public string DiscordUsername { get; set; } //At the moment of add to Db
    public string[] Platforms { get; set; } = [];
}