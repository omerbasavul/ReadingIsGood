using Microsoft.EntityFrameworkCore;
using ReadingIsGood.BuildingBlocks.ApplicationContext.Domain.Entities;

namespace ReadingIsGood.BuildingBlocks.ApplicationContext.Infrastructure.Context;

public interface IApplicationContext : IDisposable, IAsyncDisposable
{
    DbContext GetDbContext();

    ValueTask<int> SaveChangesAsync<TKey>(CancellationToken cancellationToken = default);

    string GetDatabaseProvider();

    DbSet<TEntity> Set<TEntity, TKey>() where TEntity : BaseEntity<TKey>, IEntity;
}