using Microsoft.Extensions.DependencyInjection;
using one_time_access_code_extractor.Repositories;
using one_time_access_code_extractor.Repositories.Auth;
using one_time_access_code_extractor.Repositories.Base;
using one_time_access_code_extractor.Repositories.GoogleAuth;
using one_time_access_code_extractor.Services;
using one_time_access_code_extractor.Services.Auth;
using one_time_access_code_extractor.Services.Auth.GoogleAuth;
using one_time_access_code_extractor.Services.Discord;
using one_time_access_code_extractor.Services.Gmail;


namespace one_time_access_code_extractor.Configuration;

public static class DependencyInjection
{
    public static void RegisterApplicationServices(this IServiceCollection services)
    {
        // Register repositories
        services.AddScoped(typeof(ITokenBaseRepository<>), typeof(TokenBaseRepository<>));
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IGoogleTokenRepository, GoogleTokenTokenRepository>();

        // Register services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        services.AddScoped<IApiDisneyPlusGmailService, ApiDisneyPlusGmailService>();
        services.AddScoped<IGmailServiceHelper, GmailServiceHelper>();
        services.AddScoped<IDiscordDisneyPlusGmailService, DiscordDisneyPlusGmailService>();

        services.AddScoped<IDiscordWhitelistRepository, DiscordWhitelistRepository>();
        services.AddScoped<IDiscordWhitelistService, DiscordWhitelistService>();

        // Discord
        services.AddSingleton<IDiscordMessageHandler, DiscordMessageHandler>();
        services.AddSingleton<IDiscordInitializerService, DiscordInitializerService>();
    }
}