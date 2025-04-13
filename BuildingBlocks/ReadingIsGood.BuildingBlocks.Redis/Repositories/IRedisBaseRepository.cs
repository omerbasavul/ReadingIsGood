using System.Linq.Expressions;
using Redis.OM.Searching;

namespace ReadingIsGood.BuildingBlocks.Redis.Repositories;

public interface IRedisBaseRepository<T> where T : notnull
{
    public Task CreateIndexAsync();

    public Task<Ulid> AddAsync(T entity);

    public Task<T?> GetByIdAsync(string id);

    public Task DeleteAsync(T deletedEntity);

    public Task UpdateAsync(T updatedEntity);

    public Task<IList<T>> GetAllAsync();

    public IRedisCollection<T> AsQueryable();

    public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> expression);

    public Task<bool> ExistsAsync(Expression<Func<T, bool>> expression);

    public Task<int> CountAsync();
}