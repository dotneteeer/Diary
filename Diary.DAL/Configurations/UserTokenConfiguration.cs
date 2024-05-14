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
                RefereshToken = "REFRESH_TOKENREFRESH_TOKENREFRESH_TOKENREFRESH_TOKENREFRESH_TOKEN",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
                UserId = 1
            }
        });
        //
    }
}