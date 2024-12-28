using AutoMapper;
using Diary.Application.Resources;
using Diary.Domain.Dto.Report;
using Diary.Domain.Entity;
using Diary.Domain.Enum;
using Diary.Domain.Extensions;
using Diary.Domain.Helpers;
using Diary.Domain.Interfaces.Repositories;
using Diary.Domain.Interfaces.Services;
using Diary.Domain.Interfaces.Validations;
using Diary.Domain.Result;
using Diary.Domain.Settings;
using Diary.Producer.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Polly;

namespace Diary.Application.Services;

public class ReportService : IReportService
{
    private readonly IDistributedCache _distributedCache;
    private readonly IMapper _mapper;
    private readonly IMessageProducer _messageProducer;
    private readonly IValidator<PageReportDto> _pageReportDtoValidator;
    private readonly IOptions<RabbitMqSettings> _rabbitMqOptions;
    private readonly IBaseRepository<Report> _reportRepository;
    private readonly IReportValidator _reportValidator;
    private readonly IBaseRepository<User> _userRepository;

    public ReportService(IBaseRepository<Report> reportRepository, IBaseRepository<User> userRepository,
        IReportValidator reportValidator, IMapper mapper, IOptions<RabbitMqSettings> rabbitMqOptions,
        IMessageProducer messageProducer, IDistributedCache distributedCache,
        IValidator<PageReportDto> pageReportDtoValidator)
    {
        _reportRepository = reportRepository;
        _userRepository = userRepository;
        _reportValidator = reportValidator;
        _mapper = mapper;
        _rabbitMqOptions = rabbitMqOptions;
        _messageProducer = messageProducer;
        _distributedCache = distributedCache;
        _pageReportDtoValidator = pageReportDtoValidator;
    }

    /// <inheritdoc />
    public async Task<CollectionResult<ReportDto>> GetReportsAsync(long userId, PageReportDto? dto = null)
    {
        ReportDto[] reports;
        reports = await _reportRepository.GetAll()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.LastEditedAt)
            .Select(x =>
                new ReportDto(x.Id, x.Name, x.Description, x.LastEditedAt.ToLongUtcString()))
            .ToArrayAsync();

        var totalCount = reports.Length;

        if (!reports.Any())
        {
            return new CollectionResult<ReportDto>
            {
                ErrorMessage = ErrorMessage.ReportsNotFound,
                ErrorCode = (int)ErrorCodes.ReportsNotFound
            };
        }

        dto ??= new PageReportDto();

        var currentPage = dto.PageNumber;
        var currentPageSize = dto.PageSize;

        if (!(await _pageReportDtoValidator.ValidateAsync(dto)).IsValid)
        {
            currentPage = 1;
            currentPageSize = reports.Length;
        }

        reports = reports.Skip((currentPage - 1) *
                               currentPageSize).Take(currentPageSize).ToArray();

        return new CollectionResult<ReportDto>
        {
            Data = reports,
            Count = reports.Length,
            TotalCount = totalCount
        };
    }

    /// <inheritdoc />
    public Task<BaseResult<ReportDto>> GetReportByIdAsync(long id)
    {
        ReportDto? report;
        report = _distributedCache.GetObject<ReportDto>(RedisNameHelper.GetReportRedisName(id))
                 ?? _reportRepository.GetAll()
                     .AsEnumerable()
                     .Select(x =>
                         new ReportDto(x.Id, x.Name, x.Description,
                             x.LastEditedAt.ToLongUtcString()))
                     .FirstOrDefault(x => x.Id == id);

        if (report == null)
        {
            return Task.FromResult(new BaseResult<ReportDto>()
            {
                ErrorMessage = ErrorMessage.ReportNotFound,
                ErrorCode = (int)ErrorCodes.ReportNotFound
            });
        }

        _distributedCache.SetObject(RedisNameHelper.GetReportRedisName(id), report,
            new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5)));

        return Task.FromResult(new BaseResult<ReportDto>()
        {
            Data = report
        });
    }

    /// <inheritdoc />
    public async Task<BaseResult<ReportDto>> CreateReportAsync(CreateReportDto dto)
    {
        var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.UserId);
        var result = _reportValidator.CreateValidator(user);
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

        var policy = Policy.Handle<Exception>()
            .WaitAndRetryAsync(5, i => TimeSpan.FromSeconds(Math.Pow(i, 2)));

        await policy.ExecuteAsync(() =>
        {
            _messageProducer.SendMessage(report, _rabbitMqOptions.Value.RoutingKey,
                _rabbitMqOptions.Value.ExchangeKey);
            return Task.CompletedTask;
        });


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

        _reportRepository.Remove(report!);
        await _reportRepository.SaveChangesAsync();

        await _distributedCache.RemoveAsync(RedisNameHelper.GetReportRedisName(report));

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

        report!.Name = dto.Name;
        report.Description = dto.Description;

        var updatedReport = _reportRepository.Update(report);
        await _reportRepository.SaveChangesAsync();

        var reportDto = _mapper.Map<ReportDto>(updatedReport);

        _distributedCache.SetObject($"Report_{reportDto.Id}", reportDto,
            new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5)));

        return new BaseResult<ReportDto>
        {
            Data = reportDto
        };
    }
}