using Core.Persistence.Entity;
using System.Linq.Expressions;

namespace Core.Persistence.Repositories.Abstracts;

public interface IEfBaseRepository<TEntity, in TId> where TEntity : BaseEntity<TId>, new()
{
    IQueryable<TEntity> GetAll();
    ValueTask<TEntity?> GetByIdAsync(TId id);
    IQueryable<TEntity> Where(Expression<Func<TEntity, bool>>? predicate = null);
    ValueTask AddAsync(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}
