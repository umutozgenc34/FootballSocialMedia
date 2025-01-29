using Core.Persistence.Repositories.Abstracts;
using FootballSocialMedia.Model.Users.Entities;

namespace FootballSocialMedia.Service.JWT.Abstracts;

public interface IUserRefreshTokenService : IEfBaseRepository<UserRefreshToken, string>;
