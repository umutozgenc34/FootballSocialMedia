using Core.CrossCuttingConcerns.Response;
using FootballSocialMedia.Model.Search.Dtos;

namespace FootballSocialMedia.Service.Search.Abstracts;

public interface ISearchService
{
    Task<ServiceResult<SearchResultDto>> SearchAsync(string searchText);
    Task<ServiceResult<AutoCompleteDto>> AutoCompleteAsync(string prefix);
}
