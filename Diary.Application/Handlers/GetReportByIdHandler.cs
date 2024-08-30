using Diary.Application.Queries;
using Diary.Domain.Dto.Report;
using Diary.Domain.Entity;
using Diary.Domain.Extensions;
using Diary.Domain.Interfaces.Repositories;
using MediatR;

namespace Diary.Application.Handlers;

public class GetReportByIdHandler(IBaseRepository<Report> reportRepository)
    : IRequestHandler<GetReportByIdQuery, ReportDto?>
{
    public Task<ReportDto?> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(reportRepository.GetAll()
            .AsEnumerable()
            .Select(x =>
                new ReportDto(x.Id, x.Name, x.Description,
                    x.LastEditedAt.ToLongUtcString()))
            .FirstOrDefault(x => x.Id == request.Id));
    }
}