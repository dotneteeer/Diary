using System.Reflection;
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
    private static readonly Mock<ReportValidator> MockValidator = new();
    private static readonly Mock<IOptions<RabbitMqSettings>> MockRabbitMqOptions = new();
    private static readonly Mock<Producer.Producer> MockMessageProducer = new();

    private static readonly ReportService ReportService = new(MockReportRepository.Object,
        MockUserRepository.Object,
        MockValidator.Object, Mapper, MockRabbitMqOptions.Object, MockMessageProducer.Object,
        MockDistributedCache.Object, null!);

    static ReportServiceFields()
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

    public static ReportService GetService() => ReportService;
}