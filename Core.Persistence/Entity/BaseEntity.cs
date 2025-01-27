namespace Core.Persistence.Entity;

public abstract class BaseEntity<TId>
{
    public TId Id { get; set; } = default!;
}
