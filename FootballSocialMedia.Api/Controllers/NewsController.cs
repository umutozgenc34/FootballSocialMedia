using Asp.Versioning;
using FootballSocialMedia.Model.News.Dtos.Requests;
using FootballSocialMedia.Service.Newss.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace FootballSocialMedia.Api.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
public class NewsController(INewsService newsService) : CustomBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAllNews()
       => CreateActionResult(await newsService.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetNews([FromRoute] Guid id)
        => CreateActionResult(await newsService.GetByIdAsync(id));

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateNews([FromForm] CreateNewsRequest request)
        => CreateActionResult(await newsService.CreateAsync(request));

    [HttpPut]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateNews([FromForm] UpdateNewsRequest request)
        => CreateActionResult(await newsService.UpdateAsync(request));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteNews([FromRoute] Guid id)
        => CreateActionResult(await newsService.DeleteAsync(id));
}
