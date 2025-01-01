namespace Diary.Domain.Interfaces;

public interface IAuditable
{
    public DateTime CreatedAt { get; }
    public long CreatedBy { get; }
    public DateTime? UpdatedAt { get; }
    public long? UpdatedBy { get; }
    public DateTime LastEditedAt { get; }
    public long LastEditedBy { get; }

    public void UpdateCreatedAt();
    public void UpdateUpdatedAt();
}