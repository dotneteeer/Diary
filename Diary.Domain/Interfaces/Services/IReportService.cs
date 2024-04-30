using Diary.Domain.Dto.Report;
using Diary.Domain.Result;

namespace Diary.Domain.Interfaces.Services;

public interface IReportService
{
    /// <summary>
    /// Get all reports by user's id
    /// </summary>
    /// <param name="userId">user's id</param>
    /// <returns>Returns all reports of user by its id</returns>
    Task<CollectionResult<ReportDto>> GetReportsAsync(long userId);
    
    /// <summary>
    /// Get report by its id
    /// </summary>
    /// <param name="id">report's id</param>
    /// <returns>report with the specified id </returns>
    Task<BaseResult<ReportDto>> GetReportByIdAsync(long id);
    
    /// <summary>
    /// Creates report
    /// </summary>
    /// <param name="dto">report dto to create</param>
    /// <returns>created report</returns>
    Task<BaseResult<ReportDto>> CreateReportAsync(CreateReportDto dto);

    /// <summary>
    /// Deletes report by its id
    /// </summary>
    /// <param name="id">report's id</param>
    /// <returns>deleted report</returns>
    Task<BaseResult<ReportDto>> DeleteReportAsync(long id);

    /// <summary>
    /// Update report
    /// </summary>
    /// <param name="dto">dto, which updates, contains id, name, description</param>
    /// <returns>updated report</returns>
    Task<BaseResult<ReportDto>> UpdateReportAsync(UpdateReportDto dto);
}