using Diary.Domain.Dto.Report;
using Diary.Domain.Entity;
using Diary.Domain.Interfaces.Repositories;
using Diary.Domain.Interfaces.Services;
using Diary.Domain.Result;
using HotChocolate;
using Microsoft.EntityFrameworkCore;

namespace Diary.Application.GraphQl.Queries;

public class ReportQuery
{
    [UseSorting]
    [UseFiltering]
    public async Task<CollectionResult<Report>> GetReports(long userId, PageReportDto? pageReportDto,
        [Service] IReportService reportService,
        [Service] IBaseRepository<Report> reportRepository)
    {
        var collectionDto = await reportService.GetReportsAsync(userId, pageReportDto);

        var idCollection = collectionDto.Data.Select(x => x.Id);
        var collectionReport = new CollectionResult<Report>
        {
            Data = reportRepository.GetAll().Where(x => idCollection.Contains(x.Id)),
            Count = collectionDto.Count,
            TotalCount = collectionDto.TotalCount
        };

        return collectionReport;
    }

    [UseSorting]
    [UseFiltering]
    public async Task<BaseResult<Report>> GetReport(long id, [Service] IReportService reportService,
        [Service] IBaseRepository<Report> reportRepository)
    {
        var dtoBaseResult = await reportService.GetReportByIdAsync(id);

        var reportBaseResult = new BaseResult<Report>
        {
            Data = (await reportRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dtoBaseResult.Data.Id))!,
            ErrorCode = dtoBaseResult.ErrorCode,
            ErrorMessage = dtoBaseResult.ErrorMessage
        };

        return reportBaseResult;
    }
}