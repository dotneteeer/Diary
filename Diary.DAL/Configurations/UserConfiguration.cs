using Diary.Domain.Entity;
using Diary.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.IdentityModel.Tokens;

namespace Diary.DAL.Configurations;

public class UserConfiguration:IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(user => user.Id).ValueGeneratedOnAdd();
        builder.Property(user => user.Login).HasMaxLength(100).IsRequired();
        builder.Property(user => user.Password).IsRequired();

        builder.HasMany<Report>(user => user.Reports)
            .WithOne(report=>report.User)
            .HasForeignKey(report=>report.UserId)
            .HasPrincipalKey(user=>user.Id);
        
    }
}