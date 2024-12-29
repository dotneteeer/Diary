using Diary.Domain.Entity;
using Diary.Domain.Interfaces.Repositories;
using MockQueryable.Moq;
using Moq;

namespace Diary.Tests.UnitTests.Configurations;

public static class MockRepositoriesGetter
{
    public static Mock<IBaseRepository<Report>> GetMockReportRepository()
    {
        var mock = new Mock<IBaseRepository<Report>>();
        var reports = GetReports().BuildMockDbSet();
        mock.Setup(x => x.GetAll()).Returns(() => reports.Object);
        mock.Setup(x => x.Update(It.IsAny<Report>())).Returns((Report r) => r);
        mock.Setup(x => x.CreateAsync(It.IsAny<Report>())).ReturnsAsync((Report r) => r);
        mock.Setup(x => x.Remove(It.IsAny<Report>())).Returns((Report r) => r);
        return mock;
    }

    public static Mock<IBaseRepository<User>> GetMockUserRepository()
    {
        var mock = new Mock<IBaseRepository<User>>();
        var users = GetUsers().BuildMockDbSet();
        mock.Setup(x => x.GetAll()).Returns(() => users.Object);
        mock.Setup(x => x.Update(It.IsAny<User>())).Returns((User u) => u);
        mock.Setup(x => x.CreateAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);
        mock.Setup(x => x.Remove(It.IsAny<User>())).Returns((User u) => u);
        return mock;
    }

    public static IQueryable<Report> GetReports()
    {
        var report1 = Report.Create("UnitTestReport1", "UnitTestReport1", 1);
        var report2 = Report.Create("UnitTestReport2", "UnitTestReport2", 2);

        report1.Id = 1;
        report1.UpdateCreatedAt();
        report2.Id = 2;
        report2.UpdateUpdatedAt();

        return new List<Report> { report1, report2 }.AsQueryable();
    }

    public static IQueryable<User> GetUsers()
    {
        return new List<User>
        {
            new User
            {
                Id = 1,
                Login = "UnitTestUser1",
                Password = "UnitTestPassword1",
                CreatedAt = DateTime.UtcNow,
            },
            new User
            {
                Id = 2,
                Login = "UnitTestUser2",
                Password = "UnitTestPassword2",
                CreatedAt = DateTime.UtcNow,
            },
        }.AsQueryable();
    }
}