using System.Net;

namespace Fe.Api.Common;

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

public class InvalidRequest(string error, string message)
    : ProblemException(HttpStatusCode.BadRequest, error, message);

public class NotFound(string message)
    : InvalidRequest("Not Found", message);
