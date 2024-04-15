using Diary.DAL.Interceptors;
using Diary.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Diary.DAL.DependencyInjection;

public static class DependencyInjection
{
    public static void AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(/*"PostgresSQL"*/"MSSQL");
        services.AddSingleton<DateInterceptor>();
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            //options.UseNpgsql();
            options.UseSqlServer(connectionString);
        });
        
        //services.InitRepositories();
    }

    // private static void InitRepositories(this IServiceCollection services)
    // {
    //     services.AddScoped<IBaseRepository<User>, BaseRepository<User>>();
    // }
}