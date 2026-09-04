using BookMyHall.Application.Common.Options;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookMyHall.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        // MediatR
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
        });

        // AutoMapper
        services.AddAutoMapper(config =>
        {
            config.AddMaps(assembly);
        });

        // FluentValidation
        services.AddValidatorsFromAssembly(assembly);

        // Image Processing Options
        services.Configure<ImageProcessingOptions>(options =>
        {
            var section = configuration.GetSection(
                ImageProcessingOptions.SectionName);

            options.ThumbnailWidth =
                int.TryParse(section["ThumbnailWidth"], out var width)
                    ? width
                    : 400;

            options.ThumbnailHeight =
                int.TryParse(section["ThumbnailHeight"], out var height)
                    ? height
                    : 300;

            options.ThumbnailQuality =
                int.TryParse(section["ThumbnailQuality"], out var quality)
                    ? quality
                    : 80;
        });

        return services;
    }
}