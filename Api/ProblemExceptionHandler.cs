using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Api;

public abstract class ProblemException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string Error { get; }
    public string Message { get; }

    public ProblemException(HttpStatusCode statusCode, string error, string message) : base(message)
    {
        StatusCode = statusCode;
        Error = error;
        Message = message;
    }
}

public class InvalidRequestProblem(string error, string message)
    : ProblemException(HttpStatusCode.BadRequest, error, message);

public class ValidationProblem(string message)
    : InvalidRequestProblem("Validation Error", message);

public class NotFoundProblem(string message)
    : InvalidRequestProblem("Not Found", message);

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public GlobalExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ProblemException problemException)
        {
            return await _problemDetailsService.TryWriteAsync(
                new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    Exception = exception,
                    ProblemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Type = exception.GetType().Name,
                        Title = "Internal Server Error",
                        Detail = exception.Message,
                    }
                }
            );
        }
        
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        return await _problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "Bad Request",
                    Title = problemException.Error,
                    Detail = problemException.Message,
                }
            }
        );
    }
}
