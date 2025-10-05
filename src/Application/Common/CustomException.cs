using System.Net;

namespace Application.Common
{
    public class CustomException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : Exception(message)
    {
        public HttpStatusCode StatusCode { get; } = statusCode;
    }
}
