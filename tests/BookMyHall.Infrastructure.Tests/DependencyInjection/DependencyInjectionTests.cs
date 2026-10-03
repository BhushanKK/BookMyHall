using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using BookMyHall.Application.Abstractions.Authentication;
using BookMyHall.Application.Abstractions.Security;
using BookMyHall.Infrastructure.Authentication;
using Microsoft.Extensions.FileProviders;

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

        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }

    private static IHostEnvironment CreateEnvironment()
    {
        return new TestHostEnvironment
        {
            EnvironmentName = "Testing",
            ApplicationName = "BookMyHall.Infrastructure.Tests",
            ContentRootPath = AppContext.BaseDirectory
        };
    }

    [Fact]
    public void AddInfrastructure_TestingDoesNotRegisterBackgroundWorkers()
    {
        var services = new ServiceCollection();
        services.AddInfrastructure(CreateConfiguration(), CreateEnvironment());

        services.Should().NotContain(descriptor => descriptor.ServiceType == typeof(IHostedService) &&
            descriptor.ImplementationType != null && descriptor.ImplementationType.Assembly == typeof(DependencyInjection).Assembly);
    }

    [Fact]
    public void AddInfrastructure_ProductionRegistersBackgroundWorkers()
    {
        var services = new ServiceCollection();
        var environment = CreateEnvironment();
        environment.EnvironmentName = Environments.Production;
        services.AddInfrastructure(CreateConfiguration(), environment);

        services.Should().Contain(descriptor => descriptor.ServiceType == typeof(IHostedService) &&
            descriptor.ImplementationType == typeof(BookMyHall.Infrastructure.Messaging.Consumers.VendorImageThumbnailConsumer));
    }

    [Fact]
    public void AddInfrastructure_Should_Register_All_Services()
    {
        var services = new ServiceCollection();

        var configuration = CreateConfiguration();
        var environment = CreateEnvironment();

        services.AddInfrastructure(
            configuration,
            environment);

        using var provider = services.BuildServiceProvider();

        provider
            .GetService<IPasswordHasher>()
            .Should()
            .NotBeNull();

        provider
            .GetService<IJwtTokenService>()
            .Should()
            .NotBeNull();

        provider
            .GetService<ICurrentUser>()
            .Should()
            .NotBeNull();
    }

    [Fact]
    public void AddInfrastructure_Should_Register_JwtOptions()
    {
        var services = new ServiceCollection();

        var configuration = CreateConfiguration();
        var environment = CreateEnvironment();

        services.AddInfrastructure(
            configuration,
            environment);

        using var provider = services.BuildServiceProvider();

        var options = provider
            .GetRequiredService<IOptions<JwtOptions>>();

        options.Value.Issuer
            .Should()
            .Be("BookMyHall");

        options.Value.Audience
            .Should()
            .Be("BookMyHallUsers");
    }

    [Fact]
    public void AddInfrastructure_Should_Throw_When_Issuer_Is_Missing()
    {
        var services = new ServiceCollection();

        var configuration = CreateConfiguration(
            issuer: "");

        var environment = CreateEnvironment();

        Action action = () =>
            services.AddInfrastructure(
                configuration,
                environment);

        action
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Jwt:Issuer is missing.");
    }

    [Fact]
    public void AddInfrastructure_Should_Throw_When_Audience_Is_Missing()
    {
        var services = new ServiceCollection();

        var configuration = CreateConfiguration(
            audience: "");

        var environment = CreateEnvironment();

        Action action = () =>
            services.AddInfrastructure(
                configuration,
                environment);

        action
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Jwt:Audience is missing.");
    }

    [Fact]
    public void AddInfrastructure_Should_Throw_When_SecretKey_Is_Missing()
    {
        var services = new ServiceCollection();

        var configuration = CreateConfiguration(
            secretKey: "");

        var environment = CreateEnvironment();

        Action action = () =>
            services.AddInfrastructure(
                configuration,
                environment);

        action
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Jwt:SecretKey is missing.");
    }

    [Fact]
    public void AddInfrastructure_Should_Throw_When_SecretKey_Is_TooShort()
    {
        var services = new ServiceCollection();

        var configuration = CreateConfiguration(
            secretKey: "ShortKey");

        var environment = CreateEnvironment();

        Action action = () =>
            services.AddInfrastructure(
                configuration,
                environment);

        action
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage(
                "Jwt:SecretKey must be at least 32 characters long.");
    }

    [Fact]
    public void AddInfrastructure_Should_Throw_When_AccessTokenExpiry_Is_Invalid()
    {
        var services = new ServiceCollection();

        var configuration = CreateConfiguration(
            accessTokenExpiryMinutes: 0);

        var environment = CreateEnvironment();

        Action action = () =>
            services.AddInfrastructure(
                configuration,
                environment);

        action
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage(
                "Jwt:AccessTokenExpiryMinutes must be greater than zero.");
    }

    [Fact]
    public void AddInfrastructure_Should_Throw_When_RefreshTokenExpiry_Is_Invalid()
    {
        var services = new ServiceCollection();

        var configuration = CreateConfiguration(
            refreshTokenExpiryDays: 0);

        var environment = CreateEnvironment();

        Action action = () =>
            services.AddInfrastructure(
                configuration,
                environment);

        action
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage(
                "Jwt:RefreshTokenExpiryDays must be greater than zero.");
    }

    [Fact]
    public void AddInfrastructure_Should_Throw_When_Jwt_Section_Is_Missing()
    {
        var services = new ServiceCollection();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();

        var environment = CreateEnvironment();

        Action action = () =>
            services.AddInfrastructure(
                configuration,
                environment);

        action
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage(
                "JWT configuration section is missing.");
    }

    private sealed class TestHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } =
            Environments.Development;

        public string ApplicationName { get; set; } =
            string.Empty;

        public string ContentRootPath { get; set; } =
            string.Empty;

        public IFileProvider ContentRootFileProvider
        {
            get;
            set;
        } = null!;
    }
}
