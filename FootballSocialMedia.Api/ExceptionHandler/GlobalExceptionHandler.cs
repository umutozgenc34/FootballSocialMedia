using Core.CrossCuttingConcerns.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace FootballSocialMedia.Api.ExceptionHandler;

/// <summary>
/// Yakalanmamış tüm hataları front'un beklediği formatta ({ errorMessage: [...] }) döner.
/// Eski hali sadece CloudinaryException'ı yakalıyordu, diğerlerinde boş 500 dönüyordu.
/// </summary>
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception: {Path}", httpContext.Request.Path);

        string message = exception switch
        {
            CloudinaryException => "Dosya yüklenirken bir hata oluştu (Cloudinary). Lütfen daha sonra tekrar deneyin.",
            _ when environment.IsDevelopment() => exception.Message,
            _ => "Beklenmeyen bir hata oluştu."
        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(new { errorMessage = new[] { message } }, cancellationToken);
        return true;
    }
}
