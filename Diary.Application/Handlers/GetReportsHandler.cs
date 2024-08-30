using Diary.Application.Queries;
using Diary.Domain.Dto.Report;
using Diary.Domain.Entity;
using Diary.Domain.Extensions;
using Diary.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Diary.Application.Handlers;

public class GetReportsHandler(IBaseRepository<Report> reportRepository) : IRequestHandler<GetReportsQuery, ReportDto[]>
{
    public async Task<ReportDto[]> Handle(GetReportsQuery request, CancellationToken cancellationToken)
    {
        return await reportRepository.GetAll()
            .Where(x => x.UserId == request.UserId)
            .OrderByDescending(x => x.LastEditedAt)
            .Select(x =>
                new ReportDto(x.Id, x.Name, x.Description, x.LastEditedAt.ToLongUtcString()))
            .ToArrayAsync(cancellationToken);
    }
}