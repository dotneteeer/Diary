using Diary.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Diary.Tests.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    protected readonly IReportService _reportService;
    private readonly IServiceScope _scope;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        _reportService = _scope.ServiceProvider.GetRequiredService<IReportService>();
    }
}