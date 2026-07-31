using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BookMyHall.Application.Abstractions.Authentication;
using BookMyHall.Application.Abstractions.Security;
using BookMyHall.Infrastructure.Authentication;

namespace BookMyHall.Infrastructure.Tests;

public sealed class DependencyInjectionTests
{
    private static IConfiguration CreateConfiguration(
        string issuer = "BookMyHall",
        string audience = "BookMyHallUsers",
        string secretKey = "ThisIsASuperSecretKeyWithMinimum32Characters!",
        int accessTokenExpiryMinutes = 60,
        int refreshTokenExpiryDays = 7)
    {
        var settings = new Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = issuer,
            ["Jwt:Audience"] = audience,
            ["Jwt:SecretKey"] = secretKey,
            ["Jwt:AccessTokenExpiryMinutes"] = accessTokenExpiryMinutes.ToString(),
            ["Jwt:RefreshTokenExpiryDays"] = refreshTokenExpiryDays.ToString()
        };

        return new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
    }

    [Fact]
    public void AddInfrastructure_Should_Register_All_Services()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();
        services.AddInfrastructure(configuration);
        var provider = services.BuildServiceProvider();
        provider.GetService<IPasswordHasher>().Should().NotBeNull();
        provider.GetService<IJwtTokenService>().Should().NotBeNull();
        provider.GetService<ICurrentUser>().Should().NotBeNull();
    }

    [Fact]
    public void AddInfrastructure_Should_Register_JwtOptions()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();
        services.AddInfrastructure(configuration);
        var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<JwtOptions>>();
        options.Value.Issuer.Should().Be("BookMyHall");
        options.Value.Audience.Should().Be("BookMyHallUsers");
    }

    [Fact]
    public void AddInfrastructure_Should_Throw_When_Issuer_Is_Missing()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration(issuer: "");
        Action action = () => services.AddInfrastructure(configuration);
        action.Should().Throw<InvalidOperationException>().WithMessage("Jwt:Issuer is missing.");
    }

    [Fact]
    public void AddInfrastructure_Should_Throw_When_Audience_Is_Missing()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration(audience: "");
        Action action = () => services.AddInfrastructure(configuration);
        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Jwt:Audience is missing.");
    }

    [Fact]
    public void AddInfrastructure_Should_Throw_When_SecretKey_Is_Missing()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration(secretKey: "");
        Action action = () => services.AddInfrastructure(configuration);
        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Jwt:SecretKey is missing.");
    }

    [Fact]
    public void AddInfrastructure_Should_Throw_When_SecretKey_Is_TooShort()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration(secretKey: "ShortKey");
        Action action = () => services.AddInfrastructure(configuration);
        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Jwt:SecretKey must be at least 32 characters long.");
    }

    [Fact]
    public void AddInfrastructure_Should_Throw_When_AccessTokenExpiry_Is_Invalid()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration(accessTokenExpiryMinutes: 0);
        Action action = () => services.AddInfrastructure(configuration);
        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Jwt:AccessTokenExpiryMinutes must be greater than zero.");
    }

    [Fact]
    public void AddInfrastructure_Should_Throw_When_RefreshTokenExpiry_Is_Invalid()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration(refreshTokenExpiryDays: 0);
        Action action = () => services.AddInfrastructure(configuration);
        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Jwt:RefreshTokenExpiryDays must be greater than zero.");
    }

    [Fact]
    public void AddInfrastructure_Should_Throw_When_Jwt_Section_Is_Missing()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();
        Action action = () => services.AddInfrastructure(configuration);
        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("JWT configuration section is missing.");
    }
}