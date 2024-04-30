using Diary.Domain.Dto.Report;
using FluentValidation;

namespace Diary.Application.Validation.FluentValidations.Report;

public class CreateReportValidator : AbstractValidator<CreateReportDto>
{
    public CreateReportValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.UserId).NotEmpty();//added by myself
    }
}