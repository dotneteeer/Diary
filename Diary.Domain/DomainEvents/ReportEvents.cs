using Diary.Domain.Interfaces;

namespace Diary.Domain.DomainEvents;

public record ReportCreatedDomainEvent(long ReportId) : IDomainEvent;

public record ReportUpdatedDomainEvent(long ReportId) : IDomainEvent;