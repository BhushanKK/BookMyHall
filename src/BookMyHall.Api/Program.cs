using System.Globalization;
using BookMyHall.Api.Extensions;
using BookMyHall.Api.Middleware;
using BookMyHall.Application;
using BookMyHall.Infrastructure;
using BookMyHall.Persistence;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();

builder.Services.AddOpenApi();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPersistence(builder.Configuration);

// ============================================================
// CORS
// ============================================================

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
            .WithOrigins("http://localhost:5173") //temp added hardcoded.
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ============================================================
// Localization
// ============================================================

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

// ============================================================
// Health Checks
// ============================================================

builder.Services.AddHealthChecks();

var app = builder.Build();

// ============================================================
// Middleware
// ============================================================

app.UseSerilogRequestLogging();

var localizationOptions =
    app.Services.GetRequiredService<
        IOptions<RequestLocalizationOptions>>();

app.UseRequestLocalization(localizationOptions.Value);

// ============================================================
// CORS
// IMPORTANT: Before Authentication / Authorization
// ============================================================

app.UseCors(CorsPolicyName);

// ============================================================
// Scalar
// ============================================================
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("BookMyHall API")
        .WithTheme(ScalarTheme.BluePlanet);
});

// ============================================================
// Exception Handling
// ============================================================

app.UseMiddleware<ExceptionHandlingMiddleware>();

// ============================================================
// Authentication / Authorization
// ============================================================

app.UseAuthentication();

app.UseAuthorization();

// ============================================================
// Static Files
// ============================================================

app.UseStaticFiles();

// ============================================================
// Endpoints
// ============================================================

app.MapHealthChecks("/health");

app.MapBookMyHallEndpoints();

await app.RunAsync();

public partial class Program
{
}