using Diary.Domain.Interfaces.Databases;

namespace Diary.Domain.Settings;

public class RedisSettings : IConnectionStringProvider
{
    private const int DefaultRedisPort = 6379;

    public string Url { get; set; }

    public string InstanceName { get; set; }

    public string GetConnectionString()
    {
        return $"{Url}:{DefaultRedisPort}";
    }
}