using System.Net.Http.Json;
using Diary.Domain.Dto.Token;
using Diary.Domain.Result;

namespace Diary.Tests.Extensions;

public static class ReportTestsExtensions
{
    public static async Task<string> LoginUser(this HttpClient httpClient, long userId)
    {
        var loginResult = await httpClient.PostAsJsonAsync("/api/v1/auth/login", new
        {
            login = $"Test user {userId}",
            password = $"Test user {userId} password"
        });

        loginResult.EnsureSuccessStatusCode();

        var loginResponse = await loginResult.Content.ReadFromJsonAsync<BaseResult<TokenDto>>();
        var accessToken = loginResponse!.Data.AccessToken;

        return accessToken;
    }
}