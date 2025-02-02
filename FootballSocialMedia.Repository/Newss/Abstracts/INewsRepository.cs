using Core.Persistence.Repositories.Abstracts;
using FootballSocialMedia.Model.News.Entities;

namespace FootballSocialMedia.Repository.Newss.Abstracts;

public interface INewsRepository : IEfBaseRepository<News,Guid>
{
}
