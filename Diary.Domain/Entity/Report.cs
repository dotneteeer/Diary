using Diary.Domain.Interfaces;

namespace Diary.Domain.Entity;

public class Report : IEntityId<long>, IAuditable
{
    private Report()
    {
    }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public User User { get; init; }

    public long UserId { get; init; }

    public DateTime CreatedAt { get; protected set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; protected set; }

    public long? UpdatedBy { get; set; }

    public DateTime LastEditedAt { get; protected set; }

    public long LastEditedBy { get; set; }

    public void UpdateCreatedAt()
    {
        CreatedAt = DateTime.UtcNow;
        LastEditedAt = DateTime.UtcNow;
    }

    public void UpdateUpdatedAt()
    {
        UpdatedAt = DateTime.UtcNow;
        LastEditedAt = DateTime.UtcNow;
    }

    public long Id { get; set; }

    public static Report Create(string name, string description, long userId)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name), "The name is required");

        var report = new Report
        {
            Name = name,
            Description = description,
            UserId = userId
        };

        return report;
    }

    public void Update(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name), "The name is required");

        Name = name;
        Description = description;
    }
}