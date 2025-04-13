using System.Linq.Expressions;
using Redis.OM;
using Redis.OM.Contracts;
using Redis.OM.Searching;

namespace ReadingIsGood.BuildingBlocks.Redis.Repositories;

public class RedisBaseRepository<T>(IRedisConnectionProvider provider) : IRedisBaseRepository<T> where T : notnull
{
    private readonly IRedisCollection<T> mCollection = provider.RedisCollection<T>();

    public async Task CreateIndexAsync()
    {
        var type = typeof(T);
        var conn = provider.Connection;

        if (!await conn.IsIndexCurrentAsync(type))
        {
            await conn.DropIndexAsync(type);
            await conn.CreateIndexAsync(type);
        }
    }

    public async Task<Ulid> AddAsync(T entity)
    {
        var addedEntity = await mCollection.InsertAsync(entity);

        if (addedEntity == null)
            throw new Exception("Failed to add entity to Redis.");

        if (!addedEntity.Contains(':')) return Ulid.Parse(addedEntity);

        var parts = addedEntity.Split(':');
        addedEntity = parts[^1];

        return Ulid.Parse(addedEntity);
    }

    public Task<T?> GetByIdAsync(string id) => mCollection.FindByIdAsync(id);

    public Task DeleteAsync(T deletedEntity) => mCollection.DeleteAsync(deletedEntity);

    public Task UpdateAsync(T updatedEntity) => mCollection.UpdateAsync(updatedEntity);

    public Task<IList<T>> GetAllAsync() => mCollection.ToListAsync();

    public IRedisCollection<T> AsQueryable() => mCollection;

    public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> expression) => mCollection.FirstOrDefaultAsync(expression);

    public Task<bool> ExistsAsync(Expression<Func<T, bool>> expression) => mCollection.AnyAsync(expression);

    public Task<int> CountAsync() => mCollection.CountAsync();
}