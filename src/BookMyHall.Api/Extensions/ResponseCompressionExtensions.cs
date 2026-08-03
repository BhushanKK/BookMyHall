using System.IO.Compression;
using Microsoft.AspNetCore.ResponseCompression;

namespace BookMyHall.Api.Extensions;

public static class ResponseCompressionExtensions
{
    public static IServiceCollection AddResponseCompressionConfiguration(
        this IServiceCollection services)
    {
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;

            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();

            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                new[]
                {
                    "application/json"
                });
        });

        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });

        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });

        return services;
    }

    public static IApplicationBuilder UseResponseCompressionConfiguration(
        this IApplicationBuilder app)
    {
        app.UseResponseCompression();

        return app;
    }
}