using BookMyHall.Application;
using BookMyHall.Infrastructure;
using BookMyHall.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Register Application, Infrastructure, Persistence
builder.Services
    .AddApplication()
    .AddInfrastructure()
    .AddPersistence(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "BookMyHall API Running");

app.Run();