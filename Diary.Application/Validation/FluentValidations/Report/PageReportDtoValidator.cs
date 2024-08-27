using Diary.Domain.Dto.Report;
using FluentValidation;

namespace Diary.Application.Validation.FluentValidations.Report;

public class PageReportDtoValidator : AbstractValidator<PageReportDto>
{
    public PageReportDtoValidator()
    {
        RuleFor(x => x.PageNumber).NotEmpty().GreaterThan(0);
        RuleFor(x => x.PageSize).NotEmpty().GreaterThan(0);
    }
}