using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace BookMyHall.Api.Tests;

public class BookMyHallWebApplicationFactory
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting(
            "Jwt:SecretKey",
            "BookMyHall-Test-Secret-Key-For-Automated-Tests-Only-123456789");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:SecretKey"] =
                    "BookMyHall-Test-Secret-Key-For-Automated-Tests-Only-123456789"
            });
        });
    }
}