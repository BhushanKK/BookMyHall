using System.Globalization;

using BookMyHall.Api.Endpoints.Identity;
using BookMyHall.Application;
using BookMyHall.Infrastructure;
using BookMyHall.Persistence;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

#region OpenAPI

builder.Services.AddOpenApi();

#endregion

#region Application Layers

builder.Services
    .AddApplication()
    .AddInfrastructure()
    .AddPersistence(builder.Configuration);

#endregion

#region Localization

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

    options.RequestCultureProviders = new IRequestCultureProvider[]
    {
        new AcceptLanguageHeaderRequestCultureProvider()
    };
});

#endregion

var app = builder.Build();

#region Localization Middleware

var localizationOptions = app.Services
    .GetRequiredService<IOptions<RequestLocalizationOptions>>();

app.UseRequestLocalization(localizationOptions.Value);

#endregion

#region Security

//app.UseAuthentication();
//app.UseAuthorization();

#endregion

#region OpenAPI

app.MapOpenApi();

app.MapScalarApiReference(options =>
{
    options
        .WithTitle("BookMyHall API")
        .WithTheme(ScalarTheme.BluePlanet);
});

#endregion

#region Endpoints

app.MapRoleEndpoints();

#endregion

await app.RunAsync();