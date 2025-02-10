
using Asp.Versioning;
using FootballSocialMedia.Model.Polls.Dtos.Requests;
using FootballSocialMedia.Service.Polls.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace FootballSocialMedia.Api.Controllers;
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
public class PollsController(IPollService pollService) : CustomBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAllPolls() => CreateActionResult(await pollService.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPoll([FromRoute] Guid id) => CreateActionResult(await pollService.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> CreatePoll([FromBody] CreatePollRequest request) => CreateActionResult(await pollService.CreateAsync(request));

    [HttpPost("vote/{pollOptionId:guid}")]
    public async Task<IActionResult> Vote([FromRoute] Guid pollOptionId) => CreateActionResult(await pollService.VoteAsync(pollOptionId));
}
