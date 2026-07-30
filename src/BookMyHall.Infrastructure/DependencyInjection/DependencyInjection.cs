using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using BookMyHall.Application.Abstractions.Authentication;
using BookMyHall.Application.Abstractions.Security;
using BookMyHall.Infrastructure.Authentication;
using BookMyHall.Infrastructure.Security;

namespace BookMyHall.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Password Hasher
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        // JWT Options
        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));

        var jwtOptions = configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>()
            ?? throw new InvalidOperationException("JWT configuration section is missing.");

        ValidateJwtOptions(jwtOptions);

        // JWT Token Service
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // Authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };
            });

        // Authorization
        services.AddAuthorization();

        return services;
    }

    private static void ValidateJwtOptions(JwtOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrWhiteSpace(options.Issuer))
            throw new InvalidOperationException("Jwt:Issuer is missing.");

        if (string.IsNullOrWhiteSpace(options.Audience))
            throw new InvalidOperationException("Jwt:Audience is missing.");

        if (string.IsNullOrWhiteSpace(options.SecretKey))
            throw new InvalidOperationException("Jwt:SecretKey is missing.");

        if (options.SecretKey.Length < 32)
            throw new InvalidOperationException("Jwt:SecretKey must be at least 32 characters long.");

        if (options.AccessTokenExpiryMinutes <= 0)
            throw new InvalidOperationException("Jwt:AccessTokenExpiryMinutes must be greater than zero.");

        if (options.RefreshTokenExpiryDays <= 0)
            throw new InvalidOperationException("Jwt:RefreshTokenExpiryDays must be greater than zero.");
    }
}