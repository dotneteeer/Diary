using Diary.Domain.Result;
using HotChocolate.Types;

namespace Diary.Application.GraphQl.Types.BaseTypes;

public class BaseResultType<T> : ObjectType<BaseResult<T>>
{
    protected override void Configure(IObjectTypeDescriptor<BaseResult<T>> descriptor)
    {
        descriptor.Description($"Base Result for {typeof(T).Name}");

        descriptor.Field(x => x.IsSuccess).Description("Is request was successful");
        descriptor.Field(x => x.ErrorCode).Description("The error code");
        descriptor.Field(x => x.ErrorMessage).Description("The error message");
        descriptor.Field(x => x.Data).Description($"The {typeof(T).Name} data");
    }
}