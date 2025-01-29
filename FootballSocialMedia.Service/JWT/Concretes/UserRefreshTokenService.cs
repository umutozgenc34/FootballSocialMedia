using Core.Persistence.Repositories.Concretes;
using FootballSocialMedia.Model.Users.Entities;
using FootballSocialMedia.Repository.Context;
using FootballSocialMedia.Service.JWT.Abstracts;

namespace FootballSocialMedia.Service.JWT.Concretes;

public class UserRefreshTokenService(AppDbContext context) : EfBaseRepository<AppDbContext, UserRefreshToken, string>(context), IUserRefreshTokenService;
