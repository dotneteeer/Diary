using AutoMapper;
using Diary.Application.Services;
using Diary.Application.Validation;
using Diary.Domain.Entity;
using Diary.Domain.Interfaces.Repositories;
using Diary.Domain.Settings;
using Diary.Tests.Configurations;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using MapperConfiguration = Diary.Tests.Configurations.MapperConfiguration;

namespace Diary.Tests;

public static class ReportServiceFields
{
    private static readonly Mock<IBaseRepository<Report>> MockReportRepository =
        MockRepositoriesGetter.GetMockReportRepository();

    private static readonly Mock<IBaseRepository<User>> MockUserRepository =
        MockRepositoriesGetter.GetMockUserRepository();

    private static readonly Mock<IDistributedCache> MockDistributedCache = new();
    private static readonly IMapper Mapper = MapperConfiguration.GetMapperConfiguration();
    private static readonly User User = MockRepositoriesGetter.GetUsers().FirstOrDefault()!;
    private static readonly Mock<ReportValidator> MockValidator = new();
    private static readonly Mock<IOptions<RabbitMqSettings>> MockRabbitMqOptions = new();
    private static readonly Mock<Producer.Producer> MockMessageProducer = new();

    private static readonly ReportService ReportService = new ReportService(MockReportRepository.Object,
        MockUserRepository.Object,
        MockValidator.Object, Mapper, MockRabbitMqOptions.Object, MockMessageProducer.Object,
        MockDistributedCache.Object, null);

    static ReportServiceFields()
    {
        var basePath = AppContext.BaseDirectory;
        var projectPath = Directory.GetParent(basePath).Parent.Parent.Parent.Parent.FullName;
        var configuration = new ConfigurationBuilder()
            .SetBasePath(projectPath)
            .AddJsonFile("Diary.Api/appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        var rabbitMqSettings = new RabbitMqSettings();
        configuration.GetSection("RabbitMqSettings").Bind(rabbitMqSettings);
        var options = Options.Create(rabbitMqSettings);

        MockRabbitMqOptions.Setup(o => o.Value).Returns(options.Value);
    }

    public static ReportService GetService() => ReportService;
}