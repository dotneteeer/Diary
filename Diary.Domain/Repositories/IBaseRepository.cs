namespace Diary.Domain.Interfaces.Repositories;

public interface IBaseRepository<TEntity>
{
    IQueryable<TEntity> GetAll();

    Task<TEntity> CreateAsync(TEntity entity);

    // TEntity Update(TEntity entity);
    //
    // void Remove(TEntity entity);
    //
    // Task<int> SaveChangesAsync();

    Task<TEntity> RemoveAsync(TEntity entity);
    
    Task<TEntity> UpdateAsync(TEntity entity);
}