using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Common.Interfaces.Storage;
using Microsoft.Extensions.Logging;

namespace BookMyHall.Application.Features.Venue;

internal static class VendorLogoStorage
{
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
