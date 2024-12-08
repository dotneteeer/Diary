using Diary.DAL;
using Diary.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Diary.Tests.Extensions;

internal static class PrepDb
{
    internal static void PrepPopulation(this IServiceCollection services)
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.EnsureDeleted();
        dbContext.Database.Migrate();

        dbContext.Set<User>().Add(new User
        {
            Login = "Test user 1",
            Password = "Test user password"
        });

        dbContext.SaveChanges();

        dbContext.Set<Report>().AddRange(new Report
            {
                Name = "Test report 1",
                Description = "Test report 1 description",
                UserId = 1
            },
            new Report
            {
                Name = "Test report 2",
                Description = "Test report 2 description",
                UserId = 1
            }
        );

        dbContext.SaveChanges();
    }
}