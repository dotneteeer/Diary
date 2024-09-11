using System.Runtime.CompilerServices;

namespace Diary.Domain.Extensions;

public static class ExceptionsExtensions
{
    public static void ThrowIfNullWithReferenceObj(this object? obj, object referenceObj,
        [CallerArgumentExpression("obj")] string objName = "",
        [CallerArgumentExpression("referenceObj")]
        string referenceName = "")
    {
        if (obj is null)
            throw new NullReferenceException(
                $"{objName} can not be found with {referenceName} = {referenceObj ?? "null"}");
    }
}