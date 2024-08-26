using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Diary.Domain.Extensions;

public static class DistributedCacheExtensions
{
    public static T GetObject<T>(this IDistributedCache cache, string key)
    {
        ThrowIfArgumentsNull(key);
        var value = cache.Get(key);
        return value?.Length > 0
            ? JsonSerializer.Deserialize<T>(value)
            : throw new NullReferenceException($"Object with key \"{key}\" was not found");
        ;
    }

    public static void SetObject<T>(this IDistributedCache cache, string key, T value,
        DistributedCacheEntryOptions? options = null)
    {
        ThrowIfArgumentsNull(key, value);
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value);
        cache.Set(key, bytes, options ?? new DistributedCacheEntryOptions());
    }

    public static void RefreshObject<T>(this IDistributedCache cache, string key,
        DistributedCacheEntryOptions? options = null)
    {
        if (cache.GetObject<T>(key) == null)
            throw new NullReferenceException($"Object with key \"{key}\" was not found");
        cache.Refresh(key);
    }

    private static void ThrowIfArgumentsNull(params object?[] arguments)
    {
        foreach (var argument in arguments)
        {
            ArgumentNullException.ThrowIfNull(argument, nameof(argument));
        }
    }
}