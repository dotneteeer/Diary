using Diary.Domain.DomainEvents;
using Diary.Domain.Interfaces;
using Diary.Domain.ValueObjects.Report;

namespace Diary.Domain.Entity;

public class Report : AbstractClasses.Entity, IAuditable
{
    private Report()
    {
    }


    public Name Name { get; private set; }
    //real value objects should be immutable

    public string Description { get; private set; }

    public User User { get; init; }

    public long UserId { get; init; }

    public List<IDomainEvent> DomainEvents => _domainEvents;

    public DateTime CreatedAt { get; private set; }

    public long CreatedBy { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public long? UpdatedBy { get; private set; }

    public DateTime LastEditedAt { get; private set; }

    public long LastEditedBy { get; private set; }

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

    public static Report Create(string name, string description, long userId)
    {
        ArgumentNullException.ThrowIfNull(userId);

        var report = new Report
        {
            Name = name,
            Description = description,
            UserId = userId
        };

        report.RaiseDomainEvent(new ReportCreatedDomainEvent(report.Id));

        return report;
    }

    public void Update(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name), "The name is required");

        Name = name;
        Description = description;

        RaiseDomainEvent(new ReportUpdatedDomainEvent(Id));
    }
}

//TODO DDD:
//1. User may have method to create report
//2. User should have public IReadOnlyCollection<Report> and private readonly List<Report>