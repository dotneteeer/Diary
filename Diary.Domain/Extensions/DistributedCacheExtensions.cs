using System.Text;
using Diary.Domain.Helpers;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Diary.Domain.Extensions;

public static class DistributedCacheExtensions
{
    public static T GetObject<T>(this IDistributedCache cache, string key)
    {
        ExceptionHelper.ThrowIfArgumentsNull(key);
        var value = Encoding.UTF8.GetString(cache.Get(key));
        return value?.Length > 0
            ? JsonConvert.DeserializeObject<T>(value)
            : default;
    }

    public static void SetObject<T>(this IDistributedCache cache, string key, T value,
        DistributedCacheEntryOptions? options = null)
    {
        ExceptionHelper.ThrowIfArgumentsNull(key, value);
        var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        }));
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