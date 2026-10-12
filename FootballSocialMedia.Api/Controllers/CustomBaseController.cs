using Core.CrossCuttingConcerns.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FootballSocialMedia.Api.Controllers;

/// <summary>
/// Tüm controller'lar varsayılan olarak giriş yapmış kullanıcı ister.
/// Herkese açık endpoint'ler [AllowAnonymous] ile işaretlenir (ör. AuthController).
/// </summary>
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize]
public class CustomBaseController : ControllerBase
{
    [NonAction]
    public IActionResult CreateActionResult<T>(ServiceResult<T> result)
    {
        return result.Status switch
        {
            HttpStatusCode.NoContent => NoContent(),
            HttpStatusCode.Created => Created(result.UrlAsCreated, result),
            _ => new ObjectResult(result) { StatusCode = (int)result.Status }
        };
    }

    [NonAction]
    public IActionResult CreateActionResult(ServiceResult result)
    {
        return result.Status switch
        {
            HttpStatusCode.NoContent => NoContent(),
            _ => new ObjectResult(result) { StatusCode = (int)result.Status }
        };
    }
}
