using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace one_time_access_code_extractor.Services.Discord;

public interface IDiscordInitializerService
{
    Task InitializeAsync();
}

public class DiscordInitializerService : IDiscordInitializerService
{
    private readonly ILogger<DiscordInitializerService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IDiscordMessageHandler _messageHandler;
    private DiscordClient? _discordClient;

    public DiscordInitializerService(ILogger<DiscordInitializerService> logger, IConfiguration configuration,
        IDiscordMessageHandler messageHandler, DiscordClient? discordClient = null)
    {
        _logger = logger;
        _configuration = configuration;
        _messageHandler = messageHandler;
        _discordClient = discordClient;
    }

    public async Task InitializeAsync()
    {
        var token = _configuration["Discord:Token"];

        if (string.IsNullOrEmpty(token))
        {
            _logger.LogError("Discord token is missing in configuration (Discord:Token)");
            throw new InvalidOperationException("Discord token is missing in configuration.");
        }

        _discordClient = new DiscordClient(new DiscordConfiguration
        {
            Token = token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents,
            MinimumLogLevel = LogLevel.Information
        });

        _discordClient.MessageCreated += OnMessageCreated;

        _logger.LogInformation("Connecting to Discord...");
        await _discordClient.ConnectAsync();
        _logger.LogInformation("Connected to Discord!");
    }

    private Task OnMessageCreated(DiscordClient sender, MessageCreateEventArgs e)
    {
        // Don't respond to bots
        if (e.Author.IsBot) return Task.CompletedTask;

        return _messageHandler.HandleMessageAsync(e);
    }
}