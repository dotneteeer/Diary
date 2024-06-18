using System.Reflection;
using Diary.DAL.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ILogger = Serilog.ILogger;

namespace Diary.DAL;

public sealed class ApplicationDbContext:DbContext//sealed added by rider(forbids to inherit)
{
    private readonly ILogger _logger;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options, ILogger logger):base(options)
    {
        _logger = logger;
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(_logger.Information, LogLevel.Information);
        optionsBuilder.AddInterceptors(new DateInterceptor());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}