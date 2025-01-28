using Core.CrossCuttingConcerns.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace FootballSocialMedia.Api.ExceptionHandler;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is CloudinaryException cloudinaryException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/json";

            var errorResponse = new
            {
                Message = cloudinaryException.Message,
                Details = "An error occurred while uploading to Cloudinary. Please try again later."
            };

            await httpContext.Response.WriteAsJsonAsync(errorResponse, cancellationToken);

            return true; 
        }

        return false;
    }
}
