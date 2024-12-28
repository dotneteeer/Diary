using Diary.DAL.Interceptors;
using Diary.DAL.Repositories;
using Diary.Domain.Entity;
using Diary.Domain.Interfaces.Databases;
using Diary.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Diary.DAL.DependencyInjection;

public static class DependencyInjection
{
    public static void AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgresSQL");
        services.AddSingleton<DateInterceptor>();
        services.AddDbContext<ApplicationDbContext>(options => { options.UseNpgsql(connectionString); });

        services.InitRepositories();
    }

    private static void InitRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IBaseRepository<User>, BaseRepository<User>>();
        services.AddScoped<IBaseRepository<Role>, BaseRepository<Role>>();
        services.AddScoped<IBaseRepository<UserRole>, BaseRepository<UserRole>>();
        services.AddScoped<IBaseRepository<UserToken>, BaseRepository<UserToken>>();
        services.AddScoped<IBaseRepository<Report>, BaseRepository<Report>>();
        services.Decorate<IBaseRepository<Report>, ReportCachedBaseRepository>();
    }
}