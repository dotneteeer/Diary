using Diary.Domain.Interfaces;

namespace Diary.Domain.Entity;

public class UserToken : IEntityId<long>
{
    public string RefreshToken { get; set; }

    public DateTime RefreshTokenExpiryTime { get; set; }

    public User User { get; set; }

    public long UserId { get; set; }
    public long Id { get; set; }
}