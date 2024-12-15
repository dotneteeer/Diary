namespace Diary.Domain.Settings;

public class RabbitMqSettings
{
    public string QueueName { get; set; }

    public string RoutingKey { get; set; }

    public string ExchangeKey { get; set; }

    public string HostName { get; set; }

    public int Port { get; set; }
}