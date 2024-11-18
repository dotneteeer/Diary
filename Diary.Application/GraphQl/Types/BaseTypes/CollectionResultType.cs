using Diary.Domain.Result;
using HotChocolate.Types;

namespace Diary.Application.GraphQl.Types.BaseTypes;

public class CollectionResultType<T> : ObjectType<CollectionResult<T>>
{
    protected override void Configure(IObjectTypeDescriptor<CollectionResult<T>> descriptor)
    {
        descriptor.Description($"Collection Result for {typeof(T).Name}");

        descriptor.Field(x => x.Count).Description($"Count of data returned");
        descriptor.Field(x => x.TotalCount).Description($"Total count of data that exist(s)");
    }
}