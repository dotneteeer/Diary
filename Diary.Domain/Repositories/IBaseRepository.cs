namespace Diary.Domain.Interfaces.Repositories;

public interface IBaseRepository<TEntity>
{
    IQueryable<TEntity> GetAll();

    Task<TEntity> CreateAsync(TEntity entity);

    Task<TEntity> RemoveAsync(TEntity entity);
    
    Task<TEntity> UpdateAsync(TEntity entity);
}