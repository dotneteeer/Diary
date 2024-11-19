using Diary.Domain.Dto.Report;
using Diary.Domain.Entity;
using Diary.Domain.Interfaces.Repositories;
using Diary.Domain.Interfaces.Services;
using Diary.Domain.Result;
using HotChocolate;
using Microsoft.EntityFrameworkCore;

namespace Diary.Application.GraphQl.Mutations;

public class ReportMutation
{
    public async Task<BaseResult<Report>> AddReportAsync(CreateReportDto input, [Service] IReportService reportService,
        [Service] IBaseRepository<Report> reportRepository)
    {
        var resultDto = await reportService.CreateReportAsync(input);

        var report = resultDto.Data != null
            ? await reportRepository.GetAll().FirstOrDefaultAsync(x => x.Id == resultDto.Data.Id)
            : null;
        var result = new BaseResult<Report>
        {
            Data = report,
            ErrorMessage = resultDto.ErrorMessage,
            ErrorCode = resultDto.ErrorCode
        };

        return result;
    }

    public async Task<BaseResult<Report>> UpdateReportAsync(UpdateReportDto input,
        [Service] IReportService reportService, [Service] IBaseRepository<Report> reportRepository)
    {
        var resultDto = await reportService.UpdateReportAsync(input);

        var report = resultDto.Data != null
            ? await reportRepository.GetAll().FirstOrDefaultAsync(x => x.Id == resultDto.Data.Id)
            : null;
        var result = new BaseResult<Report>
        {
            Data = report,
            ErrorMessage = resultDto.ErrorMessage,
            ErrorCode = resultDto.ErrorCode
        };

        return result;
    }

    public async Task<BaseResult> DeleteReportAsync(long id, [Service] IReportService reportService,
        [Service] IBaseRepository<Report> reportRepository)
    {
        var resultDto = await reportService.DeleteReportAsync(id);


        var result = new BaseResult
        {
            ErrorMessage = resultDto.ErrorMessage,
            ErrorCode = resultDto.ErrorCode
        };

        return result;
    }
}