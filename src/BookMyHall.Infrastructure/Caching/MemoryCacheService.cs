using Microsoft.Extensions.Caching.Memory;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Infrastructure.Caching;

public sealed class MemoryCacheService(
    IMemoryCache memoryCache) : ICacheService
{
    private readonly object _lock = new();
    private readonly HashSet<string> _cacheKeys = [];

    public Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default)
    {
        memoryCache.TryGetValue(key, out T? value);

        return Task.FromResult(value);
    }

    public Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration,
        CancellationToken cancellationToken = default)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration,
            SlidingExpiration = TimeSpan.FromMinutes(10),
            Size = 1
        };

        options.RegisterPostEvictionCallback(
            (evictedKey, _, _, _) =>
            {
                if (evictedKey is string cacheKey)
                {
                    lock (_lock)
                    {
                        _cacheKeys.Remove(cacheKey);
                    }
                }
            });

        memoryCache.Set(key, value, options);

        lock (_lock)
        {
            _cacheKeys.Add(key);
        }

        return Task.CompletedTask;
    }

    public Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        memoryCache.Remove(key);

        lock (_lock)
        {
            _cacheKeys.Remove(key);
        }

        return Task.CompletedTask;
    }

    public Task RemoveByPrefixAsync(
        string prefix,
        CancellationToken cancellationToken = default)
    {
        string[] keys;

        lock (_lock)
        {
            keys = _cacheKeys
                .Where(x => x.StartsWith(
                    prefix,
                    StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }

        foreach (var key in keys)
        {
            memoryCache.Remove(key);
        }

        lock (_lock)
        {
            foreach (var key in keys)
            {
                _cacheKeys.Remove(key);
            }
        }

        return Task.CompletedTask;
    }
}