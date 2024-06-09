using System.Text;
using Diary.Domain.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;


namespace Diary.Consumer;

public class RabbitMqListener : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IOptions<RabbitMqSettings> _options;
    private readonly ILogger _logger;

    public RabbitMqListener(IOptions<RabbitMqSettings> options, ILogger logger)
    {
        _options = options;
        _logger = logger;
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(_options.Value.QueueName, true, true, false, null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (obj, basicDeliver) =>
        {
            var content = Encoding.UTF8.GetString(basicDeliver.Body.ToArray());
            _logger.Information($"Message received: {content}");
            _channel.BasicAck(basicDeliver.DeliveryTag, false);
            
        };
        _channel.BasicConsume(_options.Value.QueueName, false, consumer);
        Dispose();
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
        base.Dispose();
    }
}