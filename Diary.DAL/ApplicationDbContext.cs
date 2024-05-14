using System.Reflection;
using Diary.DAL.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Diary.DAL;

public sealed class ApplicationDbContext:DbContext//sealed added by rider(forbids to inherit)
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new DateInterceptor());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}