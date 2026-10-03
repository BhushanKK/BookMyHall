using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace BookMyHall.Api.Tests;

public sealed class BookMyHallWebApplicationFactory
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Always run integration tests using the Testing environment.
        builder.UseEnvironment("Testing");

        builder.UseSetting(
            "Jwt:SecretKey",
            "BookMyHall-Test-Secret-Key-For-Automated-Tests-Only-123456789");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Jwt:SecretKey"] =
                        "BookMyHall-Test-Secret-Key-For-Automated-Tests-Only-123456789"
                });
        });
    }
}