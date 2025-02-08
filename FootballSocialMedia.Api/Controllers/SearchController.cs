using Asp.Versioning;
using FootballSocialMedia.Service.Elasticsearch.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace FootballSocialMedia.Api.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]

public class SearchController : CustomBaseController
{
    private readonly IElasticsearchService _elasticsearchService;

    public SearchController(IElasticsearchService elasticsearchService)
    {
        _elasticsearchService = elasticsearchService;
    }

    
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string searchText)
    {
        if (string.IsNullOrEmpty(searchText))
        {
            return BadRequest("Search text is required.");
        }

        var result = await _elasticsearchService.SearchAllAsync(searchText);

        return Ok(result);
    }

    [HttpGet("autocomplete")]
    public async Task<IActionResult> AutoComplete(string prefix)
    {
        var result = await _elasticsearchService.AutoCompleteAsync(prefix);
        return Ok(result.Suggest);
    }
}
