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

// ============================================================
// Serilog
// ============================================================
builder.AddSerilogLogging();

// ============================================================
// OpenAPI
// ============================================================
builder.Services.AddOpenApi();

// ============================================================
// Application / Infrastructure / Persistence
// ============================================================
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPersistence(builder.Configuration);

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
// Response Compression
// ============================================================
builder.Services.AddResponseCompressionConfiguration();

// ============================================================
// Build Application
// ============================================================
var app = builder.Build();

// ============================================================
// Serilog Request Logging
// ============================================================
app.UseSerilogRequestLogging();

// ============================================================
// Request Localization
// ============================================================
var localizationOptions =
    app.Services.GetRequiredService<
        IOptions<RequestLocalizationOptions>>();

app.UseRequestLocalization(localizationOptions.Value);

// ============================================================
// OpenAPI
// ============================================================
// Enabled outside Development so that Scalar/OpenAPI
// is also available when hosted through IIS in Production.
// ============================================================
app.MapOpenApi();

// ============================================================
// Scalar API Documentation
// ============================================================
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("BookMyHall API")
        .WithTheme(ScalarTheme.BluePlanet);
});

// ============================================================
// HTTPS Redirection
// ============================================================
app.UseHttpsRedirection();

// ============================================================
// Global Exception Handling
// ============================================================
app.UseMiddleware<ExceptionHandlingMiddleware>();

// ============================================================
// Authentication
// ============================================================
app.UseAuthentication();

// ============================================================
// Authorization
// ============================================================
app.UseAuthorization();

// ============================================================
// Static Files
// ============================================================
app.UseStaticFiles();

// ============================================================
// BookMyHall Endpoints
// ============================================================
app.MapBookMyHallEndpoints();

// ============================================================
// Run Application
// ============================================================
await app.RunAsync();