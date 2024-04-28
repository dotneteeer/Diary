using Diary.Domain.Entity;
using Diary.Domain.Enum;
using Diary.Domain.Result;

namespace Diary.Domain.Interfaces.Validations;

public interface IReportValidator:IBaseValidator<Report>
{ 
    /// <summary>
    /// Checking report existing
    /// Checking User by UserId
    /// </summary>
    /// <param name="report"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    BaseResult CreateValidator(Report report, User user);//??
}