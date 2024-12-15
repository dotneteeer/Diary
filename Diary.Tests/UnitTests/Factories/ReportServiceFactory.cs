using AutoMapper;
using Diary.Application.Services;
using Diary.Application.Validation;
using Diary.Domain.Entity;
using Diary.Domain.Interfaces.Repositories;
using Diary.Domain.Interfaces.Services;
using Diary.Domain.Settings;
using Diary.Producer.Interfaces;
using Diary.Tests.UnitTests.Configurations;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Moq;
using MapperConfiguration = Diary.Tests.UnitTests.Configurations.MapperConfiguration;

namespace Diary.Tests.UnitTests.Factories;

public class ReportServiceFactory
{
    public static readonly Mock<IOptions<RabbitMqSettings>> MockRabbitMqOptions = new();
    private readonly IReportService _reportService;
    public readonly IMapper Mapper = MapperConfiguration.GetMapperConfiguration();

    public readonly Mock<IDistributedCache> MockDistributedCache = new();
    public readonly Mock<IMessageProducer> MockMessageProducer = new();

    public readonly Mock<IBaseRepository<Report>> MockReportRepository =
        MockRepositoriesGetter.GetMockReportRepository();

    public readonly Mock<IBaseRepository<User>> MockUserRepository =
        MockRepositoriesGetter.GetMockUserRepository();

    public readonly Mock<ReportValidator> MockValidator = new();


    public ReportServiceFactory()
    {
        //the following rabbitmq options values are test (random) and have no relations with real ones
        MockRabbitMqOptions.Setup(o => o.Value).Returns(new RabbitMqSettings
        {
            QueueName = "queue",
            RoutingKey = "diary.topic",
            ExchangeKey = "diary.exchange",
            HostName = "localhost",
            Port = 5672
        });

        _reportService = new ReportService(MockReportRepository.Object,
            MockUserRepository.Object,
            MockValidator.Object, Mapper, MockRabbitMqOptions.Object, MockMessageProducer.Object,
            MockDistributedCache.Object, null!);
    }

    public IReportService GetReportService() => _reportService;
}