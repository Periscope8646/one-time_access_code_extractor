using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using one_time_access_code_extractor.Data;
using one_time_access_code_extractor.Middleware;

namespace one_time_access_code_extractor.Configuration;

public static class MiddlewareConfiguration
{
    public static async Task ConfigureMiddleware(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await db.Database.EnsureCreatedAsync();
        }

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseResponseCaching();
        app.UseCors("AllowFrontend");
        app.UseAuthentication();
        app.UseMiddleware<UserIdClaimMiddleware>();
        app.UseAuthorization();
        app.MapControllers();
    }
}