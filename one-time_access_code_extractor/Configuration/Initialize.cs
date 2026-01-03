using Microsoft.Extensions.DependencyInjection;
using one_time_access_code_extractor.Services;

namespace one_time_access_code_extractor.Configuration;

public static class Initialize
{
    public static void InitializeApplication(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var discordInitializer = serviceProvider.GetRequiredService<IDiscordInitializerService>();
        discordInitializer.InitializeAsync();
    }
}