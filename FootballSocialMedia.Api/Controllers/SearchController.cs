using Asp.Versioning;
using FootballSocialMedia.Service.Search.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace FootballSocialMedia.Api.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
public class SearchController(ISearchService searchService) : CustomBaseController
{
    /// <summary>Kullanıcı ve post araması (postlar: Elasticsearch, kapalıysa SQL)</summary>
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string q) => CreateActionResult(await searchService.SearchAsync(q));

    [HttpGet("autocomplete")]
    public async Task<IActionResult> AutoComplete([FromQuery] string prefix) => CreateActionResult(await searchService.AutoCompleteAsync(prefix));
}
