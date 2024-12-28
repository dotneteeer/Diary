using Diary.Domain.Entity;

namespace Diary.Domain.Helpers;

public static class RedisNameHelper
{
    public static string GetReportRedisName(Report report)
    {
        return $"Report_{report.Id}";
    }

    public static string GetReportRedisName(long reportId)
    {
        return $"Report_{reportId}";
    }
}