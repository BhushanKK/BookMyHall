using System.Globalization;
using System.IO.Compression;

using BookMyHall.Api.Extensions;
using BookMyHall.Api.Middleware;
using BookMyHall.Application;
using BookMyHall.Infrastructure;
using BookMyHall.Infrastructure.Messaging;
using BookMyHall.Persistence;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();

builder.Services.AddOpenApi();

builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddPersistence(builder.Configuration);

const string CorsPolicyName = "BookMyHallFrontend";

var allowedOrigins =
    builder.Configuration
        .GetSection("Cors:AllowedOrigins")
        .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Localization";
});

builder.Services.AddSingleton<ILocalizationService, LocalizationService>();

builder.Services.AddScoped<IMessageHelper, MessageHelper>();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo(Languages.English),
        new CultureInfo(Languages.Hindi),
        new CultureInfo(Languages.Marathi)
    };

    options.DefaultRequestCulture =
        new RequestCulture(Languages.English);

    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    options.RequestCultureProviders =
    [
        new AcceptLanguageHeaderRequestCultureProvider()
    ];
});

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
    [
        "application/json",
        "application/problem+json"
    ]);
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseSerilogRequestLogging();

var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();

app.UseRequestLocalization(localizationOptions.Value);

app.UseCors(CorsPolicyName);

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("BookMyHall API")
        .WithTheme(ScalarTheme.BluePlanet);
});

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<AuditLogMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var topology = scope.ServiceProvider
        .GetRequiredService<RabbitMqTopology>();

    await topology.ConfigureAsync();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapHealthChecks("/health");
app.MapBookMyHallEndpoints();
await app.RunAsync();