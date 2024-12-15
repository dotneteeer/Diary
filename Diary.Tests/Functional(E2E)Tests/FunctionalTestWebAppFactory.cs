using Diary.DAL;
using Diary.Domain.Settings;
using Diary.Tests.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;
using Xunit;

namespace Diary.Tests.Functional_E2E_Tests;

public class FunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("Diary")
        .WithUsername("postgres")
        .WithPassword("root")
        .Build();

    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
        .WithImage("rabbitmq:3-management")
        .WithPortBinding(5672, true)
        .WithEnvironment("RABBITMQ_DEFAULT_USER", "guest")
        .WithEnvironment("RABBITMQ_DEFAULT_PASS", "guest")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:latest")
        .WithPortBinding(6379, true)
        .Build();

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();
        await _redisContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _postgreSqlContainer.StopAsync();
        await _rabbitMqContainer.StopAsync();
        await _redisContainer.StopAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));

            var connectionString = _postgreSqlContainer.GetConnectionString();
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

            services.RemoveAll(typeof(IOptions<RabbitMqSettings>));
            services.Configure<RabbitMqSettings>(x =>
            {
                x.QueueName = "queue";
                x.RoutingKey = "diary.topic";
                x.ExchangeKey = "diary.exchange";
                x.HostName = _rabbitMqContainer.Hostname;
                x.Port = _rabbitMqContainer.GetMappedPublicPort(5672);
            });

            services.RemoveAll(typeof(IDistributedCache));
            services.AddStackExchangeRedisCache(redisCacheOptions =>
            {
                redisCacheOptions.Configuration = _redisContainer.GetConnectionString();
                redisCacheOptions.InstanceName = "local";
            });

            services.PrepPopulation();
        });
    }
}