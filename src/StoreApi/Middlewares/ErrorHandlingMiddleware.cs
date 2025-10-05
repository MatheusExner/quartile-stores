using System.Net;
using Application.Common;
using Domain.Interfaces;
using Newtonsoft.Json;

namespace StoreApi.Middlewares
{
    public class ErrorHandlingMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate next = next;

        public async Task Invoke(HttpContext context, IScopedLogService logService)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, logService);
            }
        }

        private static Task HandleExceptionAsync(
            HttpContext context,
            Exception exception,
            IScopedLogService logService
        )
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            if (exception is CustomException exception1)
                code = exception1.StatusCode;
            else if (exception is not null)
                code = HttpStatusCode.InternalServerError;

            logService.InvokeWithCustomProperties(
                action: () =>
                {
                    logService.Logger.LogError(
                        exception,
                        "Erro na execução do endpoint {Endpoint}",
                        context.Request.Path.Value
                    );
                },
                customProperties: new Dictionary<string, object>()
                {
                    { "Endpoint", context.Request.Path.Value }
                }
            );

            var json = JsonConvert.SerializeObject(
                new ErrorResponse(exception),
                Formatting.Indented
            );
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(json);
        }
    }

    public class ErrorResponse(Exception ex)
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = ex.Message;
    }
}
