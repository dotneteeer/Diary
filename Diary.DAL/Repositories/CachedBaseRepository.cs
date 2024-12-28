using Diary.Domain.Entity;
using Diary.Domain.Extensions;
using Diary.Domain.Helpers;
using Diary.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Caching.Distributed;

namespace Diary.DAL.Repositories;

public class CachedReportBaseRepository : IBaseRepository<Report>
{
    private readonly IDistributedCache _cache;
    private readonly IBaseRepository<Report> _decorated;

    public CachedReportBaseRepository(IBaseRepository<Report> decorated, IDistributedCache cache)
    {
        _decorated = decorated;
        _cache = cache;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _decorated.SaveChangesAsync();
    }

    public IQueryable<Report> GetAll()
    {
        return _decorated.GetAll();
    }

    public async Task<Report> CreateAsync(Report entity)
    {
        entity = await _decorated.CreateAsync(entity);
        _cache.SetObject(RedisNameHelper.GetReportRedisName(entity), entity,
            new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
        return entity;
    }

    public Report Update(Report entity)
    {
        entity = _decorated.Update(entity);

        _cache.SetObject(RedisNameHelper.GetReportRedisName(entity), entity,
            new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));

        return entity;
    }

    public Report Remove(Report entity)
    {
        entity = _decorated.Remove(entity);

        var key = RedisNameHelper.GetReportRedisName(entity);
        if (!Equals(_cache.GetObject<Report>(key), default(Report))) _cache.RemoveAsync(key);

        return entity;
    }
}