using System.Text.Json;
using Diary.Domain.Helpers;
using Microsoft.Extensions.Caching.Distributed;

namespace Diary.Domain.Extensions;

public static class DistributedCacheExtensions
{
    public static T GetObject<T>(this IDistributedCache cache, string key)
    {
        ExceptionHelper.ThrowIfArgumentsNull(key);
        var value = cache.Get(key);
        return value?.Length > 0
            ? JsonSerializer.Deserialize<T>(value)
            : default;
    }

    public static void SetObject<T>(this IDistributedCache cache, string key, T value,
        DistributedCacheEntryOptions? options = null)
    {
        ExceptionHelper.ThrowIfArgumentsNull(key, value);
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value);
        cache.Set(key, bytes, options ?? new DistributedCacheEntryOptions());
    }

    public static void RefreshObject<T>(this IDistributedCache cache, string key,
        DistributedCacheEntryOptions? options = null)
    {
        if (cache.GetObject<T>(key).Equals(default(T)))
            throw new ArgumentException($"Object with key \"{key}\" was not found");
        cache.Refresh(key);
    }
}