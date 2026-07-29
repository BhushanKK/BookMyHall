using Scalar.AspNetCore;
using BookMyHall.Api.Endpoints.Identity;
using BookMyHall.Application;
using BookMyHall.Infrastructure;
using BookMyHall.Persistence;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI
builder.Services.AddOpenApi();

// Application Layers
builder.Services
    .AddApplication()
    .AddInfrastructure()
    .AddPersistence(builder.Configuration);

var app = builder.Build();

// OpenAPI document
app.MapOpenApi();

// Scalar UI
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("BookMyHall API")
        .WithTheme(ScalarTheme.BluePlanet);
});

// Minimal API Endpoints
app.MapRoleEndpoints();
await app.RunAsync();