using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;

namespace one_time_access_code_extractor.Services;

public interface IDiscordMessageHandler
{
    Task HandleMessageAsync(MessageCreateEventArgs e);
}

public class DiscordMessageHandler : IDiscordMessageHandler
{
    private readonly ILogger<DiscordMessageHandler> _logger;

    public DiscordMessageHandler(ILogger<DiscordMessageHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleMessageAsync(MessageCreateEventArgs e)
    {
        if (e.Message.Content.ToLower().StartsWith("!ping"))
        {
            await e.Message.RespondAsync("Pong! 🏓");
        }

    }
}