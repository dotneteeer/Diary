using Diary.Domain.Interfaces.Repositories;

namespace Diary.DAL.Repositories;

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

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<TEntity> CreateAsync(TEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity), "Entity is null");
        }

        await _dbContext.AddAsync(entity);

        return entity;
    }

    public TEntity Update(TEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity), "Entity is null");
        }

        _dbContext.Update(entity);

        return entity;
    }

    public void Remove(TEntity entity)//method return value is void, but imo it must be TEntity just in case for other developers
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity), "Entity is null");
        }

        _dbContext.Remove(entity);


        //return entity;//method return value is void, but imo it must be TEntity just in case for other developers
    }
}