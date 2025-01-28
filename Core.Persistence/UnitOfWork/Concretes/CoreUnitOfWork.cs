using Core.Persistence.UnitOfWork.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace Core.Persistence.UnitOfWork.Concretes;

public class CoreUnitOfWork<TContext> : ICoreUnitOfWork
    where TContext : DbContext
{
    private readonly TContext _context;
    public CoreUnitOfWork(TContext context)
    {
        _context = context;
    }
    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
}