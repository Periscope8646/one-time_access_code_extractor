using Microsoft.Extensions.DependencyInjection;
using one_time_access_code_extractor.Repositories.Auth;
using one_time_access_code_extractor.Repositories.Base;
using one_time_access_code_extractor.Repositories.GoogleAuth;
using one_time_access_code_extractor.Services;
using one_time_access_code_extractor.Services.Auth;
using one_time_access_code_extractor.Services.Auth.GoogleAuth;
using one_time_access_code_extractor.Services.Gmail;


namespace one_time_access_code_extractor.Configuration;

public static class DependencyInjection
{
    public static void RegisterApplicationServices(this IServiceCollection services)
    {
        // Register repositories
        services.AddScoped(typeof(IUserTokenBaseRepository<>), typeof(UserTokenBaseRepository<>));
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IGoogleUserTokenRepository, GoogleUserTokenUserTokenRepository>();

        // Register services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        services.AddScoped<IDisneyPlusGmailService, DisneyPlusGmailService>();

        // Discord
        services.AddSingleton<IDiscordMessageHandler, DiscordMessageHandler>();
        services.AddSingleton<IDiscordInitializerService, DiscordInitializerService>();
    }
}