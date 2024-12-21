using Diary.Domain.Entity;
using Diary.Domain.Result;

namespace Diary.Domain.Interfaces.Validations;

public interface IReportValidator : IBaseValidator<Report>
{
    /// <summary>
    /// Checks User by UserId
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    BaseResult CreateValidator(User user);
}