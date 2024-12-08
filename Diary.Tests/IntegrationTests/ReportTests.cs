using Diary.Domain.Dto.Report;
using Xunit;

namespace Diary.Tests.IntegrationTests;

public class ReportTests : BaseIntegrationTest
{
    public ReportTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateReport_ShouldBe_NewReport()
    {
        //Arrange
        var createReportDto = new CreateReportDto("Test report 3", "Test report 3 description",
            1);

        //Assert
        var result = await _reportService.CreateReportAsync(createReportDto);

        //Act
        Assert.True(result.IsSuccess);
    }
}