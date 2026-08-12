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
// Health Checks
// ============================================================
// Used by IIS / GitHub Actions CD pipeline to verify that
// the BookMyHall API has started successfully.
builder.Services.AddHealthChecks();


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
// IMPORTANT
// ============================================================
// BookMyHall currently runs on HTTP :8065.
// Do NOT enable HTTPS redirection until BookMyHall
// has its own HTTPS binding/certificate.
//
// ETGS SSL on port 443 is completely independent.
//
// app.UseHttpsRedirection();


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
// Health Check
// ============================================================
// This endpoint is used by the CD pipeline:
//
// GET http://127.0.0.1:8065/health
//
// Expected response:
// HTTP 200
//
// It verifies that the ASP.NET Core application is running.
app.MapHealthChecks("/health");


// ============================================================
// BookMyHall Endpoints
// ============================================================
app.MapBookMyHallEndpoints();


// ============================================================
// Run Application
// ============================================================
await app.RunAsync();