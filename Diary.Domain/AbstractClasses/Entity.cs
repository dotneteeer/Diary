using Diary.Domain.Interfaces;

namespace Diary.Domain.AbstractClasses;

public class Entity : IEntityId<long>
{
    protected List<IDomainEvent> _domainEvents = [];
    public long Id { get; set; }
}