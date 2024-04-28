using Diary.Domain.Dto.Report;
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
    
    /// <summary>
    /// Get report by its id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>report with the specified id </returns>
    Task<BaseResult<ReportDto>> GetReportByIdAsync(long id);
    
    /// <summary>
    /// Creates report
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>created report</returns>
    Task<BaseResult<ReportDto>> CreateReportAsync(CreateReportDto dto);

    /// <summary>
    /// Deletes report by its id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<BaseResult<ReportDto>> DeleteReportAsync(long id);

    /// <summary>
    /// Update report
    /// </summary>
    /// <param name="dto">dto contains id, name, description</param>
    /// <returns></returns>
    Task<BaseResult<ReportDto>> UpdateReportAsync(UpdateReportDto dto);
}