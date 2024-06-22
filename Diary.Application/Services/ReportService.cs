using System.Globalization;
using AutoMapper;
using Diary.Application.Resources;
using Diary.Domain.Dto.Report;
using Diary.Domain.Entity;
using Diary.Domain.Enum;
using Diary.Domain.Interfaces.Repositories;
using Diary.Domain.Interfaces.Services;
using Diary.Domain.Interfaces.Validations;
using Diary.Domain.Result;
using Diary.Domain.Settings;
using Diary.Producer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Diary.Application.Services;

public class ReportService : IReportService
{
    private readonly IMessageProducer _messageProducer;
    private readonly IOptions<RabbitMqSettings> _rabbitMqOptions;
    private readonly IBaseRepository<Report> _reportRepository;
    private readonly IBaseRepository<User> _userRepository;
    private readonly IReportValidator _reportValidator;
    private readonly IMapper _mapper;

    public ReportService(IBaseRepository<Report> reportRepository, IBaseRepository<User> userRepository,
        IReportValidator reportValidator, IMapper mapper, IOptions<RabbitMqSettings> rabbitMqOptions,
        IMessageProducer messageProducer)
    {
        _reportRepository = reportRepository;
        _userRepository = userRepository;
        _reportValidator = reportValidator;
        _mapper = mapper;
        _rabbitMqOptions = rabbitMqOptions;
        _messageProducer = messageProducer;
    }

    /// <inheritdoc />
    public async Task<CollectionResult<ReportDto>> GetReportsAsync(long userId)
    {
        ReportDto[] reports;
        var enUsCulture = CultureInfo.CreateSpecificCulture("en-US");
        reports = await _reportRepository.GetAll()
            .Where(x => x.UserId == userId)
            .Select(x => new ReportDto(x.Id, x.Name, x.Description, x.CreatedAt.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss",enUsCulture)))
            .ToArrayAsync();

        if (!reports.Any())
        {
            return new CollectionResult<ReportDto>
            {
                ErrorMessage = ErrorMessage.ReportsNotFound,
                ErrorCode = (int)ErrorCodes.ReportsNotFound
            };
        }

        return new CollectionResult<ReportDto>
        {
            Data = reports,
            Count = reports.Length
        };
    }

    /// <inheritdoc />
    public Task<BaseResult<ReportDto>> GetReportByIdAsync(long id)
    {
        ReportDto? report;
        var enUsCulture = CultureInfo.CreateSpecificCulture("en-US");
        report = _reportRepository.GetAll()
            .AsEnumerable()
            .Select(x =>
                new ReportDto(x.Id, x.Name, x.Description, x.CreatedAt.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss",enUsCulture)))
            .FirstOrDefault(x => x.Id == id);

        if (report == null)
        {

            return Task.FromResult(new BaseResult<ReportDto>()
            {
                ErrorMessage = ErrorMessage.ReportNotFound,
                ErrorCode = (int)ErrorCodes.ReportNotFound
            });
        }

        return Task.FromResult(new BaseResult<ReportDto>()
        {
            Data = report
        });
    }

    /// <inheritdoc />
    public async Task<BaseResult<ReportDto>> CreateReportAsync(CreateReportDto dto)
    {
        var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.UserId);
        var result = _reportValidator.CreateValidator( user);
        if (!result.IsSuccess)
        {
            return new BaseResult<ReportDto>
            {
                ErrorMessage = result.ErrorMessage,
                ErrorCode = result.ErrorCode
            };
        }

        var report = new Report
        {
            Name = dto.Name,
            Description = dto.Description,
            UserId = user.Id
        };

        await _reportRepository.CreateAsync(report);
        await _reportRepository.SaveChangesAsync();
        
        _messageProducer.SendMessage(report, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeKey);
        
        return new BaseResult<ReportDto>
        {
            Data = _mapper.Map<ReportDto>(report)
        };
    }

    /// <inheritdoc />
    public async Task<BaseResult<ReportDto>> DeleteReportAsync(long id)
    {
        var report = await _reportRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
        var result = _reportValidator.ValidateOnNull(report);
        if (!result.IsSuccess)
        {
            return new BaseResult<ReportDto>
            {
                ErrorMessage = result.ErrorMessage,
                ErrorCode = result.ErrorCode
            };
        }

        _reportRepository.Remove(report);
        await _reportRepository.SaveChangesAsync();
        return new BaseResult<ReportDto>
        {
            Data = _mapper.Map<ReportDto>(report)
        };
    }

    public async Task<BaseResult<ReportDto>> UpdateReportAsync(UpdateReportDto dto)
    {
        var report = await _reportRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id);
        var result = _reportValidator.ValidateOnNull(report);
        if (!result.IsSuccess)
        {
            return new BaseResult<ReportDto>
            {
                ErrorMessage = result.ErrorMessage,
                ErrorCode = result.ErrorCode
            };
        }

        report.Name = dto.Name;
        report.Description = dto.Description;

        var updatedReport = _reportRepository.Update(report);
        await _reportRepository.SaveChangesAsync();

        return new BaseResult<ReportDto>
        {
            Data = _mapper.Map<ReportDto>(updatedReport)
        };
    }
}