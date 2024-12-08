using Diary.Application.Resources;
using Diary.Domain.Dto.Report;
using Diary.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Diary.Tests.IntegrationTests;

public class ReportTests : BaseReportIntegrationTest
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
        var addedReport = await _dbContext.Set<Report>().FirstOrDefaultAsync(x => x.Id == result.Data.Id);

        //Act
        Assert.True(result.IsSuccess);
        Assert.NotNull(addedReport);
    }

    [Fact]
    public async Task CreateReport_ShouldBe_UserNotFoundError_When_UserIdIsNotValid()
    {
        //Arrange
        var reportName = "Test report 4";
        var createReportDto = new CreateReportDto(reportName, "Test report 4 description",
            0);

        //Assert
        var result = await _reportService.CreateReportAsync(createReportDto);
        var addedReport = await _dbContext.Set<Report>().FirstOrDefaultAsync(x => x.Name == reportName);

        //Act
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(addedReport);
    }
}