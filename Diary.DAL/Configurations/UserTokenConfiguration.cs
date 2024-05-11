using Diary.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diary.DAL.Configurations;

public class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
{
    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.RefereshToken).IsRequired();
        builder.Property(x => x.RefreshTokenExpiryTime).IsRequired();
        
        //
        builder.HasData(new List<UserToken>
        {
            new UserToken
            {
                Id = 1,
                RefereshToken = "tyggyusgyubsdgbuvuswftgd5623r523ed4562336274t23r544623r423r",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
                UserId = 1
            }
        });
        //
    }
}