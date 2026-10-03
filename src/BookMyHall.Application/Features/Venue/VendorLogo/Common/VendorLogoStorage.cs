using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Common.Interfaces.Storage;
using Microsoft.Extensions.Logging;

namespace BookMyHall.Application.Features.Venue;

internal static class VendorLogoStorage
{
    public static string? Validate(VendorLogoUpload? logo)
    {
        if (logo is null)
        {
            return null;
        }

        var contentType = Path.GetExtension(logo.FileName).ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => null
        };
        return logo.Stream is null || logo.FileSize is <= 0 or > 5 * 1024 * 1024 || contentType is null ||
            !string.Equals(contentType, logo.ContentType, StringComparison.OrdinalIgnoreCase)
            ? "Provide a valid JPG, PNG, or WEBP logo up to 5 MB."
            : null;
    }

    public static async Task<string> UploadAsync(IR2StorageService storage, Guid vendorId, VendorLogoUpload logo,
        ILogger logger, CancellationToken cancellationToken)
    {
        var key = $"vendors/{vendorId}/logos/{Guid.NewGuid()}{Path.GetExtension(logo.FileName).ToLowerInvariant()}";
        try
        {
            if (logo.Stream.CanSeek)
            {
                logo.Stream.Position = 0;
            }
            await storage.UploadAsync(logo.Stream, key, logo.ContentType, cancellationToken);
            return key;
        }
        catch
        {
            await DeleteSafelyAsync(storage, key, logger);
            throw;
        }
    }

    public static async Task DeleteSafelyAsync(IR2StorageService storage, string? key, ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        try
        {
            await storage.DeleteAsync(key, CancellationToken.None);
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Could not delete vendor logo object {ObjectKey}.", key);
        }
    }

    public static async Task InvalidateCacheAsync(ICacheService cache, Guid vendorId, CancellationToken cancellationToken)
    {
        await cache.RemoveAsync($"{CacheKeys.Vendors}:{vendorId}", cancellationToken);
        await cache.RemoveByPrefixAsync($"{CacheKeys.VendorsPaged}:", cancellationToken);
    }
}
