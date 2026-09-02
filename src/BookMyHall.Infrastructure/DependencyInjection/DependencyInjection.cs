using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Amazon.S3;
using BookMyHall.Application.Abstractions.Authentication;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Email;
using BookMyHall.Application.Abstractions.Messaging;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Security;
using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Infrastructure.Authentication;
using BookMyHall.Infrastructure.Caching;
using BookMyHall.Infrastructure.Email;
using BookMyHall.Infrastructure.Messaging;
using BookMyHall.Infrastructure.Messaging.Consumers;
using BookMyHall.Infrastructure.Options;
using BookMyHall.Infrastructure.Security;
using BookMyHall.Infrastructure.Storage.CloudflareR2;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Options;
using BookMyHall.Shared.Configuration;

namespace BookMyHall.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<ITokenHasher, Sha256TokenHasher>();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<GoogleOptions>(configuration.GetSection(GoogleOptions.SectionName));

        var jwtOptions = configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>()
            ?? throw new InvalidOperationException("JWT configuration section is missing.");

        ValidateJwtOptions(jwtOptions);

        services.AddScoped<IJwtTokenService, JwtTokenService>();

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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        try
                        {
                            var userIdClaim = context.Principal?
                                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                            var tokenVersionClaim = context.Principal?
                                .FindFirst(CustomClaimTypes.TokenVersion)?.Value;

                            if (!Guid.TryParse(userIdClaim, out var userId) || !int.TryParse(tokenVersionClaim, out var tokenVersion))
                            {
                                context.Fail("Invalid token.");
                                return;
                            }

                            var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

                            var user = await userRepository.GetByIdAsync(userId, context.HttpContext.RequestAborted);

                            if (user is null)
                            {
                                context.Fail("User not found.");
                                return;
                            }

                            if (!user.IsActive)
                            {
                                context.Fail("User is inactive.");
                                return;
                            }

                            if (user.TokenVersion != tokenVersion)
                            {
                                context.Fail("Token has been revoked.");
                                return;
                            }
                        }
                        catch
                        {
                            context.Fail("Authentication validation failed.");
                        }
                    }
                };
            });

        services.AddAuthorization();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IClientInfoService, ClientInfoService>();

        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
        services.Configure<FrontendOptions>(configuration.GetSection(FrontendOptions.SectionName));

        services.AddScoped<IEmailSender, SmtpEmailSender>();
        services.AddScoped<IEmailTemplateService, EmailTemplateService>();

        services.Configure<CloudflareR2Options>(configuration.GetSection(CloudflareR2Options.SectionName));

        services.AddSingleton<IAmazonS3>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<CloudflareR2Options>>().Value;

            var config = new AmazonS3Config
            {
                ServiceURL = options.Endpoint,
                ForcePathStyle = true,
                AuthenticationRegion = options.Region
            };

            return new AmazonS3Client(options.AccessKeyId, options.SecretAccessKey, config);
        });

        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));

        services.AddScoped<IMessagePublisher, RabbitMqMessagePublisher>();

        services.AddSingleton<RabbitMqTopology>();

        services.AddHostedService<UserRegistrationConsumer>();
        services.AddHostedService<PasswordChangedConsumer>();
        services.AddHostedService<PasswordResetConsumer>();
        services.AddHostedService<PasswordResetSuccessConsumer>();
        services.AddHostedService<EmailVerificationConsumer>();
        services.AddHostedService<EmailVerifiedConsumer>();

        services.AddScoped<IR2StorageService, CloudflareR2StorageService>();

        services.AddMemoryCache();

        services.AddSingleton<ICacheService, MemoryCacheService>();

        return services;
    }

    private static void ValidateJwtOptions(JwtOptions options)
    {
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