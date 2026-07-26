using Microsoft.EntityFrameworkCore;

using Privat2Ynab.Application.Interfaces.Persistence;
using Privat2Ynab.Domain;

namespace Privat2Ynab.Infrastructure.Persistence;

internal sealed class Repository(Privat2YnabDbContext dbContext) :
    IRepository
{
    public async Task<IReadOnlyList<TEntity>> ListAsync<TEntity>(CancellationToken cancellationToken = default)
        where TEntity : class, IEntity =>
        (await dbContext.Set<TEntity>().AsNoTracking().ToListAsync(cancellationToken)).AsReadOnly();

    public Task<TEntity?> GetAsync<TEntity>(int id, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity =>
        dbContext.Set<TEntity>().AsNoTracking().SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<TEntity> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        var entry = await dbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return (await GetAsync<TEntity>(entry.Entity.Id, cancellationToken))!;
    }

    public async Task UpdateAsync<TEntity>(int id, Action<TEntity> update, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        var entity = await dbContext.Set<TEntity>().SingleAsync(e => e.Id == id, cancellationToken);
        update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync<TEntity>(int id, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        var entity = await dbContext.Set<TEntity>().SingleAsync(e => e.Id == id, cancellationToken);
        dbContext.Set<TEntity>().Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}