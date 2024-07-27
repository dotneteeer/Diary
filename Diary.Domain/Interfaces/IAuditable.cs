namespace Diary.Domain.Interfaces;

public interface IAuditable
{
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }
    public DateTime LastEditedAt { get; set; }
    public long LastEditedBy { get; set; }
}