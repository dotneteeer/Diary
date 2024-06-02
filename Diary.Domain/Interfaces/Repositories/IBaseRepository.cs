using Diary.Domain.Interfaces.Databases;

namespace Diary.Domain.Interfaces.Repositories;

public interface IBaseRepository<TEntity> : IStateSaveChanges
{
    IQueryable<TEntity> GetAll();
    

    Task<TEntity> CreateAsync(TEntity entity);
    
    TEntity Update(TEntity entity); 

    void Remove(TEntity entity);//method return value is void, but imo it must be TEntity just in case for other developers
}