using Diary.Domain.Interfaces;

namespace Diary.Domain.Entity;

public class User : IEntityId<long>, IAuditable
{
    public string Login { get; set; }

    public string Password { get; set; }

    public List<Report> Reports { get; set; }

    public List<Role> Roles { get; set; }

    public UserToken UserToken { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public DateTime LastEditedAt { get; set; }

    public long LastEditedBy { get; set; }
    public long Id { get; set; }
}