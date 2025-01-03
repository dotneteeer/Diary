namespace Diary.Domain.Interfaces.Databases;

public interface IConnectionStringProvider
{
    public string GetConnectionString();
}