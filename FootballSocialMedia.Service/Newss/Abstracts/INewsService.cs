using Core.CrossCuttingConcerns.Response;
using FootballSocialMedia.Model.News.Dtos.Requests;
using FootballSocialMedia.Model.News.Dtos.Responses;

namespace FootballSocialMedia.Service.Newss.Abstracts;

public interface INewsService
{
    Task<ServiceResult<NewsDto>> CreateAsync(CreateNewsRequest request);
    Task<ServiceResult> DeleteAsync(Guid id);
    Task<ServiceResult<List<NewsDto>>> GetAllAsync();
    Task<ServiceResult<NewsDto>> GetByIdAsync(Guid id);
    Task<ServiceResult> UpdateAsync(UpdateNewsRequest request);

}
