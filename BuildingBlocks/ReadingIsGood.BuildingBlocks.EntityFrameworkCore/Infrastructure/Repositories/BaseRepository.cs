using System.Runtime.CompilerServices;
using ReadingIsGood.BuildingBlocks.ApplicationContext.Domain.Entities;
using ReadingIsGood.BuildingBlocks.ApplicationContext.Infrastructure.Context;
using ReadingIsGood.BuildingBlocks.ApplicationContext.Infrastructure.Enum;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;

namespace ReadingIsGood.BuildingBlocks.EntityFrameworkCore.Infrastructure.Repositories;

public class BaseRepository<TEntity, TKey>(IApplicationContext context) : IBaseRepository<TEntity, TKey> where TEntity : AuditEntity<TKey>, IEntity
{
    private readonly DbSet<TEntity> _set = context.Set<TEntity, TKey>();

    #region Query

    public Task<TEntity?> GetByIdAsync(TKey id, bool isActive = true, bool isDeleted = false, bool tracking = false, CancellationToken cancellationToken = default) =>
        (tracking ? _set : _set.AsNoTracking())
        .FirstOrDefaultAsync(x => x.Id.Equals(id) && x.IsActive == isActive && x.IsDeleted == isDeleted, cancellationToken);

    public Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> expression, bool tracking = false, CancellationToken cancellationToken = default) =>
        (tracking ? _set : _set.AsNoTracking())
        .FirstOrDefaultAsync(expression, cancellationToken);

    public async Task<(List<TEntity>, PaginationResult)> GetListAsync(Expression<Func<TEntity, bool>> expression, int pageNumber = 1, int pageSize = 10, bool tracking = false, CancellationToken cancellationToken = default)
    {
        var set = (tracking ? _set : _set.AsNoTracking())
            .Where(expression);

        var totalResultCount = await set.LongCountAsync(cancellationToken: cancellationToken);

        var resultSet = await set
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (resultSet, PaginationResult.Create(pageNumber, totalResultCount, pageSize));
    }

    public async Task<(List<TEntity>, PaginationResult)> GetListAsync(bool isActive = true, bool isDeleted = false, int pageNumber = 1, int pageSize = 10, bool tracking = false, CancellationToken cancellationToken = default)
    {
        var set = (tracking ? _set : _set.AsNoTracking())
            .Where(x => x.IsActive == isActive && x.IsDeleted == isDeleted);

        var totalResultCount = await set.LongCountAsync(cancellationToken: cancellationToken);

        var resultSet = await set.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (resultSet, PaginationResult.Create(pageNumber, totalResultCount, pageSize));
    }

    public Task<int> CountAsync(Expression<Func<TEntity, bool>> expression, bool tracking = false, CancellationToken cancellationToken = default) =>
        (tracking ? _set : _set.AsNoTracking())
        .CountAsync(expression, cancellationToken);

    public Task<List<TEntity>> GetByDateRangeAsync(DateTime? start, DateTime? end, bool isActive = true, bool isDeleted = false, bool tracking = false, CancellationToken cancellationToken = default) =>
        (tracking ? _set : _set.AsNoTracking())
        .Where(x => x.IsActive == isActive && x.IsDeleted == isDeleted && (!start.HasValue || x.CreatedDate >= start.Value) && (!end.HasValue || x.CreatedDate <= end.Value))
        .ToListAsync(cancellationToken);

    public Task<bool> IsExistAsync(Expression<Func<TEntity, bool>> expression) => _set.AsNoTracking().AnyAsync(expression);

    public Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> expression, CancellationToken cancellationToken = default) => _set.SumAsync(expression, cancellationToken);

    public Task<decimal> AverageAsync(Expression<Func<TEntity, decimal>> expression, CancellationToken cancellationToken = default) => _set.AverageAsync(expression, cancellationToken);

    public Task<List<IGrouping<TKey, TEntity>>> GetGroupedEntitiesAsync(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TKey>> keySelector, Expression<Func<TEntity, object>>? orderBy = null, bool ascending = true, bool tracking = false, CancellationToken cancellationToken = default)
    {
        var query = (tracking ? _set : _set.AsNoTracking()).Where(filter);

        if (orderBy != null)
            query = ascending
                ? query.OrderBy(orderBy)
                : query.OrderByDescending(orderBy);

        return query.GroupBy(keySelector).ToListAsync(cancellationToken);
    }

    public Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> expression, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, int pageNumber = 1, int pageSize = 10, bool tracking = false, CancellationToken cancellationToken = default)
    {
        var query = (tracking ? _set : _set.AsNoTracking()).Where(expression);

        if (orderBy != null)
            query = orderBy(query);

        return query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
    }

    #endregion

    #region Command

    #region Add

    public async Task<int> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        try
        {
            await _set.AddAsync(CreateExtension(entity), cancellationToken);
            return await context.SaveChangesAsync<TKey>(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new($"An error occurred during the operation: {ex.Message}", ex);
        }
    }

    public async Task<int> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);

        try
        {
            await _set.AddRangeAsync(CreateExtension(entities), cancellationToken);
            return await context.SaveChangesAsync<TKey>(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new($"An error occurred during the operation: {ex.Message}");
        }
    }

    #endregion

    #region Delete

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _set.FindAsync([id, cancellationToken], cancellationToken: cancellationToken);

            ArgumentNullException.ThrowIfNull(response);

            response = DeleteExtension(response);

            _set.Update(response);

            return await context.SaveChangesAsync<TKey>(cancellationToken) > 0;
        }
        catch (Exception ex)
        {
            throw new($"An error occurred during the delete operation: {ex.Message}");
        }
    }

    public async Task<int> DeleteRangeAsync(TKey[] ids, CancellationToken cancellationToken = default)
    {
        try
        {
            var entities = await _set.Where(x => ids.Contains(x.Id)).ToArrayAsync(cancellationToken);

            ArgumentNullException.ThrowIfNull(entities);

            entities = DeleteExtension(entities);

            _set.UpdateRange(entities);

            return await context.SaveChangesAsync<TKey>(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new($"An error occurred during the delete operation: {ex.Message}");
        }
    }

    public async Task<int> HardDeleteRangeAsync(TKey[] ids, CancellationToken cancellationToken = default)
    {
        try
        {
            var entities = _set.Where(x => ids.Contains(x.Id));

            ArgumentNullException.ThrowIfNull(entities);

            _set.RemoveRange(entities);

            return await context.SaveChangesAsync<TKey>(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new($"An error occurred during the delete operation: {ex.Message}");
        }
    }

    #endregion

    #region Passive

    public async Task<bool> PassiveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _set.FindAsync([id, cancellationToken], cancellationToken: cancellationToken);

            ArgumentNullException.ThrowIfNull(response);

            response = PassiveExtension(response);

            _set.Update(response);

            return await context.SaveChangesAsync<TKey>(cancellationToken) > 0;
        }
        catch (Exception ex)
        {
            throw new($"An error occurred during the delete operation: {ex.Message}");
        }
    }

    public async Task<int> BulkPassiveAsync(TKey[] ids, CancellationToken cancellationToken = default)
    {
        try
        {
            var entities = await _set.Where(x => ids.Contains(x.Id)).ToArrayAsync(cancellationToken);

            ArgumentNullException.ThrowIfNull(entities);

            entities = PassiveExtension(entities);

            _set.UpdateRange(entities);

            return await context.SaveChangesAsync<TKey>(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new($"An error occurred during the delete operation: {ex.Message}");
        }
    }

    #endregion

    #region Update

    public async Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        try
        {
            _set.Update(UpdateExtension(entity));
            return await context.SaveChangesAsync<TKey>(cancellationToken) > 0;
        }
        catch (Exception ex)
        {
            throw new($"An error occurred during the delete operation: {ex.Message}");
        }
    }

    public async Task<int> UpdateRangeAsync(TEntity[] entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);

        try
        {
            _set.UpdateRange(UpdateExtension(entities));
            return await context.SaveChangesAsync<TKey>(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new($"An error occurred during the delete operation: {ex.Message}");
        }
    }

    #endregion

    #region Private

    #region Create

    private static TEntity CreateExtension(TEntity entity) => CreateEntityStatus(ref entity);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static TEntity[] CreateExtension(IEnumerable<TEntity> entities) => entities.Select(entity => CreateEntityStatus(ref entity)).ToArray();

    #endregion

    #region Update

    private static TEntity UpdateExtension(TEntity entity) => UpdateEntityStatus(ref entity);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static TEntity[] UpdateExtension(IEnumerable<TEntity> entities) => entities.Select(entity => UpdateEntityStatus(ref entity)).ToArray();

    #endregion

    #region Passive

    private static TEntity PassiveExtension(TEntity entity) => PassiveEntityStatus(ref entity);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static TEntity[] PassiveExtension(IEnumerable<TEntity> entities) => entities.Select(entity => PassiveEntityStatus(ref entity)).ToArray();

    #endregion

    #region Delete

    private static TEntity DeleteExtension(TEntity entity) => DeleteEntityStatus(ref entity);

    private static TEntity[] DeleteExtension(IEnumerable<TEntity> entities) => entities.Select(entity => DeleteEntityStatus(ref entity)).ToArray();

    #endregion

    private static TEntity CreateEntityStatus(ref TEntity entity)
    {
        entity.CustomState = CustomEntityState.Added;

        return entity;
    }

    private static TEntity UpdateEntityStatus(ref TEntity entity)
    {
        entity.CustomState = CustomEntityState.Updated;

        return entity;
    }

    private static TEntity PassiveEntityStatus(ref TEntity entity)
    {
        entity.CustomState = CustomEntityState.Passived;
        return entity;
    }

    private static TEntity DeleteEntityStatus(ref TEntity entity)
    {
        entity.CustomState = CustomEntityState.Deleted;

        return entity;
    }

    #endregion

    #endregion

    public void SetOriginalRowVersion(TEntity entity, uint rowVersion)
    {
        ArgumentNullException.ThrowIfNull(rowVersion);

        var entry = context.GetDbContext().Entry(entity);
        entry.Property(nameof(AuditEntity<TKey>.RowVersion)).OriginalValue = rowVersion;
    }


    public void ClearChangeTracker() => context.GetDbContext().ChangeTracker.Clear();

    public DbSet<TEntity> Set() => _set;

    public void Dispose() => context.Dispose();
}