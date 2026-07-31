using System.Globalization;
using BookMyHall.Api.Endpoints.Identity;
using BookMyHall.Api.Endpoints.Master;
using BookMyHall.Api.Endpoints.Role;
using BookMyHall.Api.Extensions;
using BookMyHall.Api.Middleware;
using BookMyHall.Application;
using BookMyHall.Application.Features.Master;
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

    options.DefaultRequestCulture = new RequestCulture(Languages.English);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders =
    [
        new AcceptLanguageHeaderRequestCultureProvider()
    ];
});

var app = builder.Build();
app.UseSerilogRequestLogging();
var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(localizationOptions.Value);
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("BookMyHall API")
            .WithTheme(ScalarTheme.BluePlanet);
    });
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapRoleEndpoints();
app.MapUserEndpoints();
app.MapAuthenticationEndpoints();
app.MapStateEndpoints();
app.MapAmenityEndpoints();
app.MapAreaEndpoints();
app.MapCancellationPolicyEndpoints();
app.MapCityEndpoints();
await app.RunAsync();
