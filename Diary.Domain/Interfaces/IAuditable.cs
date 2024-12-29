namespace Diary.Domain.Interfaces;

public interface IAuditable
{
    public DateTime CreatedAt { get; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; }
    public long? UpdatedBy { get; set; }
    public DateTime LastEditedAt { get; }
    public long LastEditedBy { get; set; }

    public void UpdateCreatedAt();
    public void UpdateUpdatedAt();
}