using Diary.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diary.DAL.Configurations;

public class UserConfiguration:IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Login).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Password).IsRequired();

        builder.HasMany<Report>(x => x.Reports)
            .WithOne(report=>report.User)
            .HasForeignKey(report=>report.UserId)
            .HasPrincipalKey(x=>x.Id);
        builder.HasMany(x => x.Roles)
            .WithMany(x => x.Users)
            .UsingEntity<UserRole>(
                x=>x.HasOne<Role>().WithMany().HasForeignKey(y=>y.RoleId),
                x=>x.HasOne<User>().WithMany().HasForeignKey(y=>y.UserId)
                );
        
        
        //

        builder.HasData(new List<User>
        {
            new()
            {
                Id = 1,
                Login = "1stUser",
                Password = "1stUserPassword"
            }
        });
        //
    }
}