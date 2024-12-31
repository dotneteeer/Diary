using Diary.Domain.Interfaces;
using Diary.Domain.ValueObjects.Report;

namespace Diary.Domain.Entity;

public class Report : IEntityId<long>, IAuditable
{
    private Name _name;

    private Report()
    {
    }

    public string FullName { get; private set; }

    public Name Name
    {
        get
        {
            _name = new Name(FullName);
            return _name;
        }
        set
        {
            _name = value;
            FullName = _name.ToString();
        }
    }
    //real value objects should be immutable

    public string Description { get; set; }

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
        ArgumentNullException.ThrowIfNull(userId);

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