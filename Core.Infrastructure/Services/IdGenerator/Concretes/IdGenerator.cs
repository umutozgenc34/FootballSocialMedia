using Core.Infrastructure.Services.IdGenerator.Abstracts;
using MassTransit;

namespace Core.Infrastructure.Services.IdGenerator.Concretes;

public class IdGenerator : IIdGenerator
{
    public Guid GenerateId()
    {
        return NewId.NextSequentialGuid();
    }
}
