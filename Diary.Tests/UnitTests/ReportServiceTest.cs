using Diary.Application.Resources;
using Diary.Domain.Dto.Report;
using Diary.Domain.Entity;
using Diary.Tests.UnitTests.Configurations;
using Diary.Tests.UnitTests.Factories;
using Diary.Tests.UnitTests.TestDataClasses;
using FluentAssertions;
using Moq;
using Xunit;

namespace Diary.Tests.UnitTests;

public class ReportServiceTest
{
    public static IEnumerable<object[]> GetReportData() => new List<object[]>
    {
        new object[] { 1, true },
        new object[] { -1, false },
    };

    [Trait("Category", "Unit")]
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GetReport_ShouldBe_NotNull_WithGivenId(long id)
    {
        //Arrange
        var reportService = new ReportServiceFactory().GetReportService();
        //Act
        var result = await reportService.GetReportByIdAsync(id);

        //Assert
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Theory]
    [MemberData(nameof(GetReportData))]
    public async Task GetReport_ShouldBe_NotNull_WithGivenIdAndExpectedResult(long id, bool isSuccessExpected)
    {
        //Arrange
        var reportService = new ReportServiceFactory().GetReportService();
        //Act
        var result = await reportService.GetReportByIdAsync(id);

        //Assert
        if (isSuccessExpected)
        {
            Assert.NotNull(result.Data);
        }
        else
        {
            Assert.False(result.IsSuccess);
        }
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetReport_ShouldBe_NotNull()
    {
        //Arrange
        var reportService = new ReportServiceFactory().GetReportService();
        //Act
        var result = await reportService.GetReportByIdAsync(1);

        //Assert
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetReport_ShouldBe_UserNotFoundError_When_UserIdIsIncorrect()
    {
        //Arrange
        var reportService = new ReportServiceFactory().GetReportService();

        //Act
        var result = await reportService.GetReportByIdAsync(-1);

        //Assert
        Assert.False(result.IsSuccess);
        result.ErrorMessage.Should().Be(ErrorMessage.ReportNotFound);
    }

    [Trait("Category", "Unit")]
    [Theory]
    [ClassData(typeof(ReportCreateTestData))]
    public async Task CreateReport_ShouldBe_NewReport(string description)
    {
        //Arrange
        var user = MockRepositoriesGetter.GetUsers().FirstOrDefault();
        var createReportDto = new CreateReportDto("UnitTestReport3", description, user.Id);
        var reportService = new ReportServiceFactory().GetReportService();

        //Act
        var result = await reportService.CreateReportAsync(createReportDto);

        //Assert
        Assert.True(result.IsSuccess);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteReport_ShouldBe_Return_TrueSuccess()
    {
        // Arrange
        var report = MockRepositoriesGetter.GetReports().FirstOrDefault();
        var reportService = new ReportServiceFactory().GetReportService();

        // Act
        var result = await reportService.DeleteReportAsync(report.Id);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateReport_ShouldBe_Return_NewData_For_Report()
    {
        // Arrange
        var report = MockRepositoriesGetter.GetReports().FirstOrDefault();
        var updateReportDto = new UpdateReportDto(report.Id, "UnitTest New name for report",
            "UnitTest New description for report");
        var reportServiceFactory = new ReportServiceFactory();
        var reportService = reportServiceFactory.GetReportService();

        // Act
        var result = await reportService.UpdateReportAsync(updateReportDto);

        // Assert
        Assert.True(result.IsSuccess);
        reportServiceFactory.MockReportRepository.Verify(x => x.Update(It.IsAny<Report>()), Times.Once);
    }
}