using Core.Persistence.Entity;
using Core.Persistence.Repositories.Abstracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Core.Persistence.Repositories.Concretes;

public class EfBaseRepository<TContext,TEntity,TId>(TContext context) : IEfBaseRepository<TEntity,TId>
    where TEntity : BaseEntity<TId>,new()
    where TContext : DbContext
{
    protected TContext Context { get; } = context;

    public IQueryable<TEntity> GetAll() => Context.Set<TEntity>().AsNoTracking();

    public ValueTask<TEntity?> GetByIdAsync(TId id) => Context.Set<TEntity>().FindAsync(id);

    public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>>? predicate = null)
    {
        return predicate is null
            ? GetAll()
            : Context.Set<TEntity>().Where(predicate);
    }

    public async ValueTask AddAsync(TEntity entity) => await Context.Set<TEntity>().AddAsync(entity);

    public void Update(TEntity entity) => Context.Set<TEntity>().Update(entity);

    public void Delete(TEntity entity) => Context.Set<TEntity>().Remove(entity);
}
