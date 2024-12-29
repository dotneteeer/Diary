using System.Security.Cryptography;
using System.Text;
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


        dbContext.Set<User>().AddRange(new User
            {
                Login = "Test user 1",
                Password = HashPassword("Test user 1 password"),
            },
            new User
            {
                Login = "Test user 2",
                Password = HashPassword("Test user 2 password")
            }
        );

        dbContext.SaveChanges();

        dbContext.Set<Report>().AddRange(Report.Create
            (
                "Test report 1",
                "Test report 1 description",
                1
            ),
            Report.Create
            (
                "Test report 2",
                "Test report 2 description",
                1
            )
        );

        dbContext.SaveChanges();
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}