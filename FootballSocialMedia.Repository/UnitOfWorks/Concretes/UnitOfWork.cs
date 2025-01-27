using Core.Persistence.UnitOfWork.Concretes;
using FootballSocialMedia.Repository.Context;
using FootballSocialMedia.Repository.UnitOfWorks.Abstracts;

namespace FootballSocialMedia.Repository.UnitOfWorks.Concretes;

public class UnitOfWork(AppDbContext context) : CoreUnitOfWork<AppDbContext>(context), IUnitOfWork
{
}
