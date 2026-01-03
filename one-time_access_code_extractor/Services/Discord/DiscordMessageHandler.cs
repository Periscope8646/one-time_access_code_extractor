using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using one_time_access_code_extractor.ConstValues;
using one_time_access_code_extractor.Services.Gmail;

namespace one_time_access_code_extractor.Services.Discord;

public interface IDiscordMessageHandler
{
    Task HandleMessageAsync(MessageCreateEventArgs e);
}

public class DiscordMessageHandler : IDiscordMessageHandler
{
    private readonly ILogger<DiscordMessageHandler> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private const string DisneyPlusThumbnailUrl = "https://static-assets.bamgrid.com/product/disneyplus/favicons/favicon-32x32-aurora.b8575e743ddc30b7e34ed4792fe2851e.png";
    public DiscordMessageHandler(ILogger<DiscordMessageHandler> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task HandleMessageAsync(MessageCreateEventArgs e)
    {
        if (e.Message.Content.StartsWith("!ping", StringComparison.CurrentCultureIgnoreCase))
        {
            _logger.LogInformation("Received ping request");
            await e.Message.RespondAsync("Pong! 🏓");
        }

        if (e.Message.Content.StartsWith($"!{Platforms.DisneyPlus}", StringComparison.CurrentCultureIgnoreCase))
        {
            _logger.LogInformation("Received request for Disney+ access code");

            using var scope = _scopeFactory.CreateScope();
            var discordWhitelistService = scope.ServiceProvider.GetRequiredService<IDiscordWhitelistService>();
            var isWhitelisted = await discordWhitelistService.IsUserWhitelistedAsync(e.Message.Author.Id, Platforms.DisneyPlus);

            if (isWhitelisted)
            {
                _logger.LogInformation("User is whitelisted, performing search");
                await SendAccessCode(e);
            }
            else
            {
                _logger.LogInformation("User is not whitelisted, ignoring request");
            }
        }

    }

    private async Task SendAccessCode(MessageCreateEventArgs e)
    {
        var loadingEmbed = new DiscordEmbedBuilder()
            .WithTitle("Searching for Disney+ Code")
            .WithDescription("Fetching data, please wait... 🔄")
            .WithColor(DiscordColor.Gray);

        var message = await e.Message.RespondAsync(embed: loadingEmbed.Build());

        using var scope = _scopeFactory.CreateScope();
        var disneyPlusGmailService = scope.ServiceProvider.GetRequiredService<IDiscordDisneyPlusGmailService>();
        var result = await disneyPlusGmailService.GetAccessCode();

        var embed = new DiscordEmbedBuilder();

        if (result.Found)
        {
            embed.WithTitle("Disney+ Access Code Found!")
                .WithThumbnail(DisneyPlusThumbnailUrl) // optional
                .WithDescription($"**Code:** `{result.Code}`")
                .WithColor(DiscordColor.Blue)
                .AddField("Status", "✅ Success", true)
                .AddField("Received At", result.ReceivedAt?.ToString("g") ?? "Unknown", true)
                .WithFooter("Requested via Disney+ Extractor")
                .WithTimestamp(DateTime.Now);
        }
        else
        {
            embed.WithTitle("Disney+ Access Code Not Found")
                .WithDescription(result.Message)
                .WithColor(DiscordColor.Red)
                .WithFooter("Please try again in a moment");
        }
        await message.ModifyAsync(m => m.Embed = embed.Build());
    }
}