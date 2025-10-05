using Microsoft.Extensions.Logging;

namespace Domain.Interfaces
{
    public interface ILogService
    {
        ILogger Logger { get; }
        Task InvokeWithCustomPropertiesAsync(
            Func<Task> function,
            Dictionary<string, object> customProperties
        );
        void InvokeWithCustomProperties(Action action, Dictionary<string, object> customProperties);
        void EnrichDiagnosticContext(string propertyName, string value);
        Task<HttpResponsePayload> LogRequestAsync(
            Task<HttpResponseMessage> requestTask,
            bool logRequestBody = true,
            bool logResponseBody = true
        );
    }

    public class HttpResponsePayload
    {
        public HttpResponseMessage HttpResponse { get; set; }
        public string StringContent { get; set; }
    }
}
