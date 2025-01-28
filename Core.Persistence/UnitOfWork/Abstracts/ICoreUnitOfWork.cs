namespace Core.Persistence.UnitOfWork.Abstracts;

public interface ICoreUnitOfWork
{
    Task<int> SaveChangesAsync();
}
