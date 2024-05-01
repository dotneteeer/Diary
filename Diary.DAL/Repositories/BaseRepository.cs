using Diary.DAL;
using Diary.Domain.Interfaces.Repositories;

namespace EF_Core.DAL.Repositories;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    private readonly ApplicationDbContext _dbContext;

    public BaseRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public IQueryable<TEntity> GetAll()
    {
        return _dbContext.Set<TEntity>();
    }

    public async Task<TEntity> CreateAsync(TEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException("Entity is null");
        }
        
        await _dbContext.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        
        return entity;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException("Entity is null");
        }
        
        _dbContext.Update(entity);
        await _dbContext.SaveChangesAsync();
        
        return entity;
    }

    public async Task<TEntity> RemoveAsync(TEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException("Entity is null");
        }
        
        _dbContext.Remove(entity);
        await _dbContext.SaveChangesAsync();
        
        return entity;
    }
    
}