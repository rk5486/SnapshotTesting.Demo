using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Fe.Api.Common;

public class ProblemExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly ILogger<ProblemExceptionHandler> _logger;

    public ProblemExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<ProblemExceptionHandler> logger)
    {
        _problemDetailsService = problemDetailsService;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        //TODO Log error
        
        if (exception is ProblemException problemException)
        {
            if (problemException is NotFound)
            {
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                return await _problemDetailsService.TryWriteAsync(
                    new ProblemDetailsContext
                    {
                        HttpContext = httpContext,
                        Exception = exception,
                        ProblemDetails = new ProblemDetails
                        {
                            Status = StatusCodes.Status404NotFound,
                            Type = "https://www.rfc-editor.org/rfc/rfc9110#name-404-not-found",
                            Title = problemException.Error,
                            Detail = problemException.Message,
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
                        Type = "https://www.rfc-editor.org/rfc/rfc9110#name-400-bad-request",
                        Title = problemException.Error,
                        Detail = problemException.Message,
                    }
                }
            );
        }
        
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return await _problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "https://www.rfc-editor.org/rfc/rfc9110#name-500-internal-server-error",
                    Title = "Internal Server Error!",
                    Detail = "An unexpected error has occurred.",
                    Extensions = { { "note", "See application log for stack trace." } },
                }
            }
        );
    }
}
