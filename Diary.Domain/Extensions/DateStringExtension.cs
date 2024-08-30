namespace Diary.Domain.Extensions;

public static class DateStringExtension
{
    public static string ToLongUtcString(this DateTime dateTime)
    {
        return dateTime.ToString("(UTC): " + "dd.MM.yyyy HH:mm");
    }
}