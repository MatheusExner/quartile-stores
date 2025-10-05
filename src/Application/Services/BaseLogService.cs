using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Application.Services
{
    public class BaseLogService(
        ILogger<BaseLogService> logger,
        Serilog.IDiagnosticContext diagnosticContext
        ) : ILogService
    {
        public ILogger Logger { get; } = logger;
        private const string _messageTemplate = "Integração Logger - {Message}";
        private readonly Serilog.IDiagnosticContext _diagnosticContext = diagnosticContext;

        public async Task InvokeWithCustomPropertiesAsync(
            Func<Task> function,
            Dictionary<string, object> customProperties
        )
        {
            var pushedProperties = PushProperties(customProperties);
            try
            {
                await Task.Run(function);
            }
            finally
            {
                DisposeProperties(pushedProperties);
            }
        }

        public void InvokeWithCustomProperties(
            Action action,
            Dictionary<string, object> customProperties
        )
        {
            var pushedProperties = PushProperties(customProperties);
            try
            {
                action.Invoke();
            }
            finally
            {
                DisposeProperties(pushedProperties);
            }
        }

        public async Task<HttpResponsePayload> LogRequestAsync(
            Task<HttpResponseMessage> requestTask,
            bool logRequestBody = true,
            bool logResponseBody = true
        )
        {
            var requestBody = "";
            var responseBody = "";
            var dateTimeStarted = DateTime.UtcNow;

            var httpResponse = await requestTask;

            var dateTimeFinished = DateTime.UtcNow;
            if (logResponseBody && httpResponse.Content != null)
            {
                responseBody = await httpResponse.Content.ReadAsStringAsync();
            }
            if (logRequestBody && httpResponse.RequestMessage.Content != null)
            {
                requestBody = await httpResponse.RequestMessage.Content.ReadAsStringAsync();
            }

            LogRequestProperties(
                httpResponse,
                requestBody,
                responseBody,
                dateTimeStarted,
                dateTimeFinished
            );

            return new HttpResponsePayload()
            {
                HttpResponse = httpResponse,
                StringContent = responseBody
            };
        }

        private void LogRequestProperties(
            HttpResponseMessage httpResponse,
            string requestBody,
            string responseBody,
            DateTime dateTimeStarted,
            DateTime dateTimeFinished
        )
        {
            var requestUri = httpResponse.RequestMessage.RequestUri?.ToString();
            var elapsed = dateTimeFinished - dateTimeStarted;

            InvokeWithCustomProperties(
                action: () =>
                {
                    Logger.LogInformation(_messageTemplate, $"Handled {requestUri}");
                },
                customProperties: new Dictionary<string, object>()
                {
                    { "RequestUri", requestUri },
                    { "RequestMethod", httpResponse.RequestMessage.Method.ToString() },
                    { "RequestBody", requestBody },
                    { "ResponseBody", responseBody },
                    { "StatusCode", (int)httpResponse.StatusCode },
                    { "DateTimeRequestStarted", dateTimeStarted.ToString("o") },
                    { "DateTimeRequestFinished", dateTimeFinished.ToString("o") },
                    { "Elapsed", elapsed.TotalMilliseconds }
                }
            );
        }

        private IDisposable[] PushProperties(Dictionary<string, object> customProperties)
        {
            return customProperties.Select(p => LogContext.PushProperty(p.Key, p.Value)).ToArray();
        }

        private void DisposeProperties(IDisposable[] pushedProperties)
        {
            foreach (var pushedProperty in pushedProperties)
            {
                pushedProperty.Dispose();
            }
        }

        public void EnrichDiagnosticContext(string propertyName, string value)
        {
            _diagnosticContext.Set(propertyName, value);
        }
    }
}
