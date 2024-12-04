using Xunit;

namespace Diary.Tests.TestDataClasses;

public class ReportCreateTestData : TheoryData<string>
{
    public ReportCreateTestData()
    {
        var description = string.Empty;
        for (int i = 0; i < 40; i++)
        {
            description += (char)Random.Shared.Next(1, 100);
        }

        Add(description);
    }
}