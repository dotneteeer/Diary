using System.Text;
using Diary.Domain.Settings;
using Diary.Producer.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Diary.Producer;

public class Producer : IMessageProducer
{
    private readonly RabbitMqSettings _rabbitMqSettings;

    public Producer(IOptions<RabbitMqSettings> options)
    {
        _rabbitMqSettings = options.Value;
    }

    public void SendMessage<T>(T message, string routingKey, string? exchange = default)
    {
        var factory = new ConnectionFactory { HostName = _rabbitMqSettings.HostName, Port = _rabbitMqSettings.Port };
        var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        var json = JsonConvert.SerializeObject(message, Formatting.Indented, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });

        var body = Encoding.UTF8.GetBytes(json);
        channel.BasicPublish(exchange, routingKey, body: body);
    }
}