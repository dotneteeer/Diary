namespace Diary.Domain.Interfaces.Databases;

public interface IStateSaveChanges
{
    Task<int> SaveChangesAsync();
}