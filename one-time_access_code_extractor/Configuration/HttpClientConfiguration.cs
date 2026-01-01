using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using one_time_access_code_extractor.ConstValues;

namespace one_time_access_code_extractor.Configuration;

public static class HttpClientConfiguration
{
    public static void ConfigureHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddHttpClient(HttpClients.GoogleOAuthClient, client =>
        {
            client.Timeout = TimeSpan.FromSeconds(60);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });
    }
}