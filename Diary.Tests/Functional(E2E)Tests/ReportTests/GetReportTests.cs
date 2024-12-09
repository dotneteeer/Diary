using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Diary.Domain.Dto.Report;
using Diary.Domain.Result;
using Diary.Tests.Extensions;
using Xunit;

namespace Diary.Tests.Functional_E2E_Tests.ReportTests;

public class GetReportTests : BaseReportFunctionalTest
{
    public GetReportTests(FunctionalTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetReports_ShouldBe_Reports()
    {
        //Arrange
        const long userId = 1;
        var pageReportDto = new PageReportDto();

        //Act
        var accessToken = await HttpClient.LoginUser(userId);

        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var result = await HttpClient.GetAsync(
            $"/api/v1.0/Report/reports/{userId}?PageNumber={pageReportDto.PageNumber}&PageSize={pageReportDto.PageSize}");
        var response = await result.Content.ReadFromJsonAsync<CollectionResult<ReportDto>>();

        //Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.True(response!.IsSuccess);
        Assert.Equal(2, response.Count);
    }

    [Fact]
    public async Task GetReports_ShouldBe_ReportsNotFound_When_UserIdIsInvalid()
    {
        //Arrange
        const long userId = 2;
        var pageReportDto = new PageReportDto();

        //Act
        var accessToken = await HttpClient.LoginUser(userId);

        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var result = await HttpClient.GetAsync(
            $"/api/v1.0/Report/reports/{userId}?PageNumber={pageReportDto.PageNumber}&PageSize={pageReportDto.PageSize}");
        var response = await result.Content.ReadFromJsonAsync<CollectionResult<ReportDto>>();

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.False(response!.IsSuccess);
        Assert.Equal(0, response.Count);
        Assert.Null(response.Data);
    }
}