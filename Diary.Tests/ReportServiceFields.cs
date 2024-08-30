using AutoMapper;
using Diary.Application.Commands;
using Diary.Application.Queries;
using Diary.Application.Services;
using Diary.Application.Validation;
using Diary.Domain.Dto.Report;
using Diary.Domain.Entity;
using Diary.Domain.Extensions;
using Diary.Domain.Interfaces.Repositories;
using Diary.Domain.Settings;
using Diary.Tests.Configurations;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
    private static readonly Mock<ReportValidator> MockReportValidator = new();
    private static readonly Mock<IOptions<RabbitMqSettings>> MockRabbitMqOptions = new();
    private static readonly Mock<Producer.Producer> MockMessageProducer = new();
    private static readonly Mock<IMediator> MockMediator = new();
    private static readonly Mock<IValidator<PageReportDto>> MockPageReportDtoValidator = new();

    private static readonly ReportService ReportService = new ReportService(MockReportRepository.Object,
        MockUserRepository.Object,
        MockReportValidator.Object, Mapper, MockRabbitMqOptions.Object, MockMessageProducer.Object,
        MockDistributedCache.Object, MockPageReportDtoValidator.Object, MockMediator.Object);

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

        //Setup
        MockRabbitMqOptions.Setup(o => o.Value).Returns(options.Value);

        MockMediator.Setup(x => x.Send(It.IsAny<GetReportsQuery>(), It.IsAny<CancellationToken>()))
            .Returns((GetReportsQuery request, CancellationToken cancellationToken) =>
                MockReportRepository.Object.GetAll()
                    .Where(x => x.UserId == request.UserId)
                    .OrderByDescending(x => x.LastEditedAt)
                    .Select(x =>
                        new ReportDto(x.Id, x.Name, x.Description, x.LastEditedAt.ToLongUtcString()))
                    .ToArrayAsync(cancellationToken));
        MockMediator.Setup(x => x.Send(It.IsAny<GetReportByIdQuery>(), It.IsAny<CancellationToken>()))
            .Returns((GetReportByIdQuery request, CancellationToken cancellationToken) =>
                Task.FromResult(MockReportRepository.Object.GetAll()
                    .AsEnumerable()
                    .Select(x =>
                        new ReportDto(x.Id, x.Name, x.Description,
                            x.LastEditedAt.ToLongUtcString()))
                    .FirstOrDefault(x => x.Id == request.Id)));
        MockMediator.Setup(x => x.Send(It.IsAny<CreateReportCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CreateReportCommand command, CancellationToken cancellationToken) =>
            {
                var report = new Report
                {
                    Name = command.Name,
                    Description = command.Description,
                    UserId = command.UserId
                };

                MockReportRepository.Object.CreateAsync(report).Wait();
                MockReportRepository.Object.SaveChangesAsync().Wait();
                return report;
            });

        MockPageReportDtoValidator.Setup(v => v.ValidateAsync(It.IsAny<PageReportDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        MockPageReportDtoValidator
            .Setup(v => v.ValidateAsync(It.Is<PageReportDto>(dto => dto.PageNumber <= 0 || dto.PageSize <= 0),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("PageNumber", "PageNumber must be greater than 0"),
                new ValidationFailure("PageSize", "PageSize must be greater than 0")
            }));
    }

    public static ReportService GetService() => ReportService;
}