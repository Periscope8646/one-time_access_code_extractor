using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using one_time_access_code_extractor.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddLogging();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.ConfigureSwagger();
builder.Services.ConfigureHttpClients(builder.Configuration);
builder.Services.ConfigurePolicies();
builder.Services.RegisterApplicationServices();

var app = builder.Build();
await app.ConfigureMiddleware();

await app.RunAsync();