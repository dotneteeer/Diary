namespace Diary.Domain.Settings;

public class RabbitMqSettings
{
    public string QueueName { get; set; }
    
    public string RoutingKey { get; set; }
    
    public string ExchangeKey { get; set; }
}