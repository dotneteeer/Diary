using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Diary.Domain.Dto.Report;
using Diary.Domain.Dto.Token;
using Diary.Domain.Result;
using Xunit;

namespace Diary.Tests.Functional_E2E_Tests.ReportTests;

public class AuthReportTests : BaseReportFunctionalTest
{
    public AuthReportTests(FunctionalTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetReports_ShouldBe_Unauthorized_When_UserIsUnauthorized()
    {
        //Arrange
        const long userId = 0;
        var pageReportDto = new PageReportDto();

        //Act
        var result = await HttpClient.GetAsync(
            $"/api/v1.0/Report/reports/{userId}?PageNumber={pageReportDto.PageNumber}&PageSize={pageReportDto.PageSize}");

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task GetReports_ShouldBe_Forbidden_When_UserIdDoesNotMatchUserIdInAccessToken()
    {
        //Arrange
        const long userId = 0;
        var pageReportDto = new PageReportDto();

        //Act: Login
        var loginResult = await HttpClient.PostAsJsonAsync("/api/v1/auth/login", new
        {
            login = "Test user 1",
            password = "Test user 1 password"
        });

        //Assert: Login
        Assert.True(loginResult.IsSuccessStatusCode);

        //Act: GetReports
        var loginResponse = await loginResult.Content.ReadFromJsonAsync<BaseResult<TokenDto>>();
        var accessToken = loginResponse!.Data.AccessToken;

        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var result = await HttpClient.GetAsync(
            $"/api/v1.0/Report/reports/{userId}?PageNumber={pageReportDto.PageNumber}&PageSize={pageReportDto.PageSize}");

        //Assert:GetReports
        Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
    }
}