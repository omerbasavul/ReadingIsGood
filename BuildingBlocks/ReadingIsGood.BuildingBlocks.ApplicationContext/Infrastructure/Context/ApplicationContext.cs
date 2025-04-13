using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ReadingIsGood.BuildingBlocks.ApplicationContext.Domain.Entities;
using ReadingIsGood.BuildingBlocks.ApplicationContext.Infrastructure.Enum;

namespace ReadingIsGood.BuildingBlocks.ApplicationContext.Infrastructure.Context;

public sealed class ApplicationContext<TContext>(TContext context, IHttpContextAccessor httpContextAccessor) : IApplicationContext where TContext : DbContext
{
    private readonly TContext _context = context ?? throw new Exception("Provided DbContext cannot be null.");
    private bool _disposed;

    public DbContext GetDbContext()
    {
        return _disposed ? throw new Exception("The DbContext has already been disposed and cannot be used.") : _context;
    }

    public DbSet<TEntity> Set<TEntity, TKey>() where TEntity : BaseEntity<TKey>, IEntity
    {
        return _context.Set<TEntity>();
    }

    public async ValueTask<int> SaveChangesAsync<TKey>(CancellationToken cancellationToken = default)
    {
        var currentUtcNow = DateTime.UtcNow;
        var currentUser = GetCurrentUserId();

        foreach (var entry in context.ChangeTracker.Entries<AuditEntity<TKey>>())
            if (entry.Entity is { } auditEntity)
                SetAuditInfo(auditEntity, currentUser, currentUtcNow, entry.Entity.CustomState);

        return await context.SaveChangesAsync(cancellationToken);
    }

    #region Extents

    public string GetDatabaseProvider()
    {
        if (_context == null) throw new Exception("DbContext instance is null.");

        var database = _context.Database ?? throw new Exception("Database property is not available.");

        return database.ProviderName ?? throw new Exception("Unknown Database Provider");
    }

    #endregion

    #region Private Methods

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetAuditInfo(IEntity entity, Guid userId, DateTime timestamp, CustomEntityState stateType)
    {
        switch (stateType)
        {
            case CustomEntityState.Added:
                AddedStateSet(entity, timestamp, userId);
                break;
            case CustomEntityState.Updated:
                UpdatedStateSet(entity, timestamp, userId);
                break;
            case CustomEntityState.Deleted:
                DeletedStateSet(entity, timestamp, userId);
                break;
            case CustomEntityState.Passived:
                PassivedStateSet(entity, timestamp, userId);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(stateType), stateType, null);
        }
    }

    private static void AddedStateSet(IEntity entity, DateTime createdDate, Guid createdBy)
    {
        entity.CreatedDate = createdDate;
        entity.CreatedBy = createdBy;

        entity.IsActive = true;
        entity.IsDeleted = false;
    }

    private static void UpdatedStateSet(IEntity entity, DateTime updatedDate, Guid updatedBy)
    {
        entity.UpdatedDate = updatedDate;
        entity.UpdatedBy = updatedBy;
    }

    private static void DeletedStateSet(IEntity entity, DateTime deletedDate, Guid deletedBy)
    {
        entity.DeletedDate = deletedDate;
        entity.DeletedBy = deletedBy;
        entity.IsDeleted = true;
    }

    private static void PassivedStateSet(IEntity entity, DateTime passivedDate, Guid passivedBy)
    {
        entity.PassiveDate = passivedDate;
        entity.PassiveBy = passivedBy;
        entity.IsActive = false;
    }


    private Guid GetCurrentUserId()
    {
        return httpContextAccessor.HttpContext != null
               && httpContextAccessor.HttpContext.Items.TryGetValue("UserId", out var userId)
            ? Guid.TryParse(userId?.ToString(), out var result)
                ? result
                : Guid.Empty
            : Guid.NewGuid();
    }

    #endregion

    #region Dispose

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing) _context.Dispose();

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }

    #endregion
}