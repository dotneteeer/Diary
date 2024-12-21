namespace Diary.Domain.Helpers;

public static class ExceptionHelper
{
    public static void ThrowIfArgumentsNull(params object?[] arguments)
    {
        foreach (var argument in arguments)
        {
            ArgumentNullException.ThrowIfNull(argument);
        }
    }
}