using Diary.DAL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Testcontainers.PostgreSql;
using Xunit;

namespace Diary.Tests.IntegrationTests;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("Diary")
        .WithUsername("postgres")
        .WithPassword("root")
        .Build();

    public Task InitializeAsync()
    {
        return _postgreSqlContainer.StartAsync();
    }

    public new Task DisposeAsync()
    {
        return _postgreSqlContainer.StopAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptor =
                services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(_postgreSqlContainer.GetConnectionString()));
        });
    }
}