using Diary.Domain.Dto.Report;
using MediatR;

namespace Diary.Application.Queries;

public class GetReportsQuery(long userId) : IRequest<ReportDto[]>
{
    public long UserId { get; set; } = userId;
}