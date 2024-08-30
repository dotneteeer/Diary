using Diary.Domain.Dto.Report;
using MediatR;

namespace Diary.Application.Queries;

public class GetReportByIdQuery(long id) : IRequest<ReportDto?>
{
    public long Id { get; set; } = id;
}