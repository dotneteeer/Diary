using Xunit;

namespace Diary.Tests.Functional_E2E_Tests;

public class BaseReportFunctionalTest : IClassFixture<FunctionalTestWebAppFactory>
{
    public BaseReportFunctionalTest(FunctionalTestWebAppFactory factory)
    {
        HttpClient = factory.CreateClient();
    }

    protected HttpClient HttpClient { get; init; }
}