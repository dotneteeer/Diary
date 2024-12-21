using Diary.Domain.Interfaces;

namespace Diary.Domain.Entity;

public class Role : IEntityId<long>
{
    public string Name { get; set; }

    public List<User> Users { get; set; }
    public long Id { get; set; }
}