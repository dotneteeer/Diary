using System.Reflection;
using AutoMapper;
using Diary.Application.Services;
using Diary.Application.Validation;
using Diary.Domain.Entity;
using Diary.Domain.Interfaces.Repositories;
using Diary.Domain.Settings;
using Diary.Tests.UnitTests.Configurations;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using MapperConfiguration = Diary.Tests.UnitTests.Configurations.MapperConfiguration;

namespace Diary.Tests.UnitTests.Factories;

public class ReportServiceFactory
{
    public static readonly Mock<IOptions<RabbitMqSettings>> MockRabbitMqOptions = new();
    private readonly ReportService _reportService;
    public readonly IMapper Mapper = MapperConfiguration.GetMapperConfiguration();

    public readonly Mock<IDistributedCache> MockDistributedCache = new();
    public readonly Mock<Producer.Producer> MockMessageProducer = new();

    public readonly Mock<IBaseRepository<Report>> MockReportRepository =
        MockRepositoriesGetter.GetMockReportRepository();

    public readonly Mock<IBaseRepository<User>> MockUserRepository =
        MockRepositoriesGetter.GetMockUserRepository();

    public readonly Mock<ReportValidator> MockValidator = new();


    static ReportServiceFactory()
    {
        #region Getting root directory

        var projectDirectory = Directory.GetParent(AppContext.BaseDirectory); //path to runtime directory
        var projectName =
            Assembly.GetExecutingAssembly().GetName().Name!.Split('.')
                .First(); //name of app if naming is of type '<AppName>.<DirectoryName>' (e. g. 'Diary.Api')
        while (projectDirectory!.Name != projectName)
            projectDirectory = projectDirectory.Parent!; //getting path to root directory

        #endregion

        var configuration = new ConfigurationBuilder()
            .SetBasePath(projectDirectory.FullName)
            .AddJsonFile(projectName + ".Api/appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        var rabbitMqSettings = new RabbitMqSettings();
        configuration.GetSection(nameof(RabbitMqSettings)).Bind(rabbitMqSettings);
        var options = Options.Create(rabbitMqSettings);

        MockRabbitMqOptions.Setup(o => o.Value).Returns(options.Value);
    }

    public ReportServiceFactory()
    {
        _reportService = new ReportService(MockReportRepository.Object,
            MockUserRepository.Object,
            MockValidator.Object, Mapper, MockRabbitMqOptions.Object, MockMessageProducer.Object,
            MockDistributedCache.Object, null!);
    }

    public ReportService GetReportService() => _reportService;
}