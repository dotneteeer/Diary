using Diary.Domain.Interfaces.Databases;

namespace Diary.Domain.Settings;

public class RabbitMqSettings : IConnectionStringProvider
{
    public string QueueName { get; set; }

    public string RoutingKey { get; set; }

    public string ExchangeKey { get; set; }

    public string HostName { get; set; }

    public int Port { get; set; }

    public string Password { get; set; } //in .net user secrets

    public string Username { get; set; } //in .net user secrets

    public string GetConnectionString()
    {
        return $"amqp://{Username ?? "guest"}:{Password ?? "guest"}@{HostName}:{Port}";
    }
}