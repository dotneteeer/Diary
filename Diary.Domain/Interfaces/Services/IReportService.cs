using Diary.Domain.Dto;
using Diary.Domain.Result;

namespace Diary.Domain.Interfaces.Services;

public interface IReportService
{
    /// <summary>
    /// Get all reports
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>Returns all reports</returns>
    Task<CollectionResult<ReportDto>> GetReportsAsync(long userId);
}