using Diary.Domain.Result;

namespace Diary.Domain.Interfaces.Validations;

public interface IBaseValidator<in T> where T : class
{
    BaseResult ValidateOnNull(T model);
}