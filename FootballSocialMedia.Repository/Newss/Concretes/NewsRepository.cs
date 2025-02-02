using Core.Persistence.Repositories.Concretes;
using FootballSocialMedia.Model.News.Entities;
using FootballSocialMedia.Repository.Context;
using FootballSocialMedia.Repository.Newss.Abstracts;

namespace FootballSocialMedia.Repository.Newss.Concretes;

public class NewsRepository(AppDbContext context) : EfBaseRepository<AppDbContext,News,Guid>(context),INewsRepository
{

}
