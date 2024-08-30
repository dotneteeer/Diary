using Diary.Application.Commands;
using Diary.Domain.Entity;
using Diary.Domain.Interfaces.Repositories;
using MediatR;

namespace Diary.Application.Handlers;

public class CreateReportHandler(IBaseRepository<Report> reportRepository)
    : IRequestHandler<CreateReportCommand, Report>
{
    public async Task<Report> Handle(CreateReportCommand request, CancellationToken cancellationToken)
    {
        var report = new Report
        {
            Name = request.Name,
            Description = request.Description,
            UserId = request.UserId
        };

        await reportRepository.CreateAsync(report);
        await reportRepository.SaveChangesAsync();
        return report;
    }
}