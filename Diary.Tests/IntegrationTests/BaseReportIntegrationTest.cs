using Diary.DAL;
using Diary.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Diary.Tests.IntegrationTests;

public abstract class BaseReportIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    protected readonly ApplicationDbContext _dbContext;
    protected readonly IReportService _reportService;
    private readonly IServiceScope _scope;

    protected BaseReportIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        _reportService = _scope.ServiceProvider.GetRequiredService<IReportService>();
        _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }
}