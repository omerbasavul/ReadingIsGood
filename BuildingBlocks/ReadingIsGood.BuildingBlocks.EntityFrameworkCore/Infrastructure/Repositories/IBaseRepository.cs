using ReadingIsGood.BuildingBlocks.ApplicationContext.Domain.Entities;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;

namespace ReadingIsGood.BuildingBlocks.EntityFrameworkCore.Infrastructure.Repositories;

public interface IBaseRepository<TEntity, TKey> : IDisposable where TEntity : AuditEntity<TKey>, IEntity
{
    Task<TEntity?> GetByIdAsync(TKey id, bool isActive = true, bool isDeleted = false, bool tracking = false, CancellationToken cancellationToken = default);

    Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> expression, bool tracking = false, CancellationToken cancellationToken = default);

    Task<(List<TEntity>, PaginationResult)> GetListAsync(Expression<Func<TEntity, bool>> expression, int pageNumber = 1, int pageSize = 10, bool tracking = false, CancellationToken cancellationToken = default);

    Task<(List<TEntity>, PaginationResult)> GetListAsync(bool isActive = true, bool isDeleted = false, int pageNumber = 1, int pageSize = 10, bool tracking = false, CancellationToken cancellationToken = default);

    Task<int> CountAsync(Expression<Func<TEntity, bool>> expression, bool tracking = false, CancellationToken cancellationToken = default);

    Task<List<TEntity>> GetByDateRangeAsync(DateTime? start, DateTime? end, bool isActive = true, bool isDeleted = false, bool tracking = false, CancellationToken cancellationToken = default);

    Task<bool> IsExistAsync(Expression<Func<TEntity, bool>> predicate);

    Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> expression, CancellationToken cancellationToken = default);

    Task<decimal> AverageAsync(Expression<Func<TEntity, decimal>> expression, CancellationToken cancellationToken = default);

    Task<List<IGrouping<TKey, TEntity>>> GetGroupedEntitiesAsync(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TKey>> keySelector,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool ascending = true,
        bool tracking = false,
        CancellationToken cancellationToken = default);

    Task<int> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<int> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<int> DeleteRangeAsync(TKey[] ids, CancellationToken cancellationToken = default);

    Task<int> BulkPassiveAsync(TKey[] ids, CancellationToken cancellationToken = default);

    Task<bool> PassiveAsync(Guid id, CancellationToken cancellationToken = default);

    Task<int> HardDeleteRangeAsync(TKey[] ids, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<int> UpdateRangeAsync(TEntity[] entities, CancellationToken cancellationToken = default);

    void SetOriginalRowVersion(TEntity entity, uint rowVersion);

    void ClearChangeTracker();

    DbSet<TEntity> Set();
}