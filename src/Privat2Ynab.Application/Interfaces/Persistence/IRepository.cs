using System.Linq.Expressions;

using Privat2Ynab.Domain;

namespace Privat2Ynab.Application.Interfaces.Persistence;

public interface IRepository
{
    Task<IReadOnlyList<TEntity>> GetAllAsync<TEntity>(CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;
    Task<IReadOnlyList<TEntity>> GetAllAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;
    Task<TEntity?> GetAsync<TEntity>(int id, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    Task<TEntity> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;
    Task UpdateAsync<TEntity>(int id, Action<TEntity> update, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;
    Task UpdateAsync<TEntity>(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;
    Task DeleteAsync<TEntity>(int id, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;
    Task DeleteAsync<TEntity>(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;
}