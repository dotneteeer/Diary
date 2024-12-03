﻿using Diary.Application.Resources;
using Diary.Domain.Dto.Report;
using Diary.Tests.Configurations;
using FluentAssertions;
using Xunit;

namespace Diary.Tests;

public class ReportServiceTest
{
    [Fact]
    public async Task GetReport_ShouldBe_NotNull()
    {
        //Arrange
        var reportService = ReportServiceFields.GetService();
        //Act
        var result = await reportService.GetReportByIdAsync(1);

        //Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetReport_ShouldBe_UserNotFoundError_When_UserIdIsIncorrect()
    {
        //Arrange
        var reportService = ReportServiceFields.GetService();

        //Act
        var result = await reportService.GetReportByIdAsync(-1);

        //Assert
        Assert.False(result.IsSuccess);
        result.ErrorMessage.Should().Be(ErrorMessage.ReportNotFound);
    }

    [Fact]
    public async Task CreateReport_ShouldBe_NewReport()
    {
        //Arrange
        var user = MockRepositoriesGetter.GetUsers().FirstOrDefault();
        var createReportDto = new CreateReportDto("UnitTestReport3", "UnitTestDescription3", user.Id);
        var reportService = ReportServiceFields.GetService();

        //Act
        var result = await reportService.CreateReportAsync(createReportDto);

        //Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DeleteReport_ShouldBe_Return_TrueSuccess()
    {
        // Arrange
        var report = MockRepositoriesGetter.GetReports().FirstOrDefault();
        var reportService = ReportServiceFields.GetService();

        // Act
        var result = await reportService.DeleteReportAsync(report.Id);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task UpdateReport_ShouldBe_Return_NewData_For_Report()
    {
        // Arrange
        var report = MockRepositoriesGetter.GetReports().FirstOrDefault();
        var updateReportDto = new UpdateReportDto(report.Id, "UnitTest New name for report",
            "UnitTest New description for report");
        var reportService = ReportServiceFields.GetService();

        // Act
        var result = await reportService.UpdateReportAsync(updateReportDto);

        // Assert
        Assert.True(result.IsSuccess);
    }
}