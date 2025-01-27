using System.Net;
using System.Text.Json.Serialization;

namespace Core.CrossCuttingConcerns.Response;

public class ServiceResult
{
    public List<string>? ErrorMessage { get; set; }
    [JsonIgnore] public HttpStatusCode Status { get; set; }
    [JsonIgnore] public bool IsSuccess => ErrorMessage == null || ErrorMessage.Count == 0;
    [JsonIgnore] public bool IsFail => !IsSuccess;
    public string? Message { get; set; }

    public static ServiceResult Success(HttpStatusCode status = HttpStatusCode.OK)
    {
        return new ServiceResult()
        {

            Status = status
        };
    }

    public static ServiceResult Success(string message, HttpStatusCode status = HttpStatusCode.OK)
    {
        return new ServiceResult()
        {
            Message = message,
            Status = status
        };
    }

    public static ServiceResult Fail(List<string> errorMessage, HttpStatusCode status = HttpStatusCode.BadRequest)
    {
        return new ServiceResult()
        {
            ErrorMessage = errorMessage,
            Status = status
        };
    }

    public static ServiceResult Fail(string errorMessage, HttpStatusCode status = HttpStatusCode.BadRequest)
    {
        return new ServiceResult()
        {
            ErrorMessage = [errorMessage],
            Status = status

        };
    }
}
public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; set; }
    [JsonIgnore] public string? UrlAsCreated { get; set; }

    public static ServiceResult<T> Success(T data, HttpStatusCode status = HttpStatusCode.OK)
    {
        return new ServiceResult<T>()
        {
            Data = data,
            Status = status
        };
    }

    public static ServiceResult<T> SuccessAsCreated(T data, string urlAsCreated)
    {
        return new ServiceResult<T>()
        {
            Data = data,
            Status = HttpStatusCode.Created,
            UrlAsCreated = urlAsCreated
        };
    }

    public static ServiceResult<T> Fail(List<string> errorMessage,
           HttpStatusCode status = HttpStatusCode.BadRequest)
    {
        return new ServiceResult<T>()
        {
            ErrorMessage = errorMessage,
            Status = status
        };
    }

    public static ServiceResult<T> Fail(string errorMessage, HttpStatusCode status = HttpStatusCode.BadRequest)
    {
        return new ServiceResult<T>()
        {
            ErrorMessage = [errorMessage],
            Status = status
        };
    }
}