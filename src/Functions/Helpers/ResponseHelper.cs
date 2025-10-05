using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Text.Json;
using Functions.Dtos;

namespace Functions.Helpers
{
    public static class ResponseHelper
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public static async Task<HttpResponseData> CreateJsonResponse<T>(
            HttpRequestData request, 
            T data, 
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var response = request.CreateResponse(statusCode);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            
            var json = JsonSerializer.Serialize(data, JsonOptions);
            await response.WriteStringAsync(json);
            
            return response;
        }

        public static async Task<HttpResponseData> CreateErrorResponse(
            HttpRequestData request,
            HttpStatusCode statusCode,
            string error,
            string message,
            string? details = null)
        {
            var errorResponse = new ErrorResponseDto
            {
                Error = error,
                Message = message,
                Details = details
            };

            return await CreateJsonResponse(request, errorResponse, statusCode);
        }

        public static async Task<HttpResponseData> CreateValidationErrorResponse(
            HttpRequestData request,
            List<string> validationErrors)
        {
            return await CreateErrorResponse(
                request,
                HttpStatusCode.BadRequest,
                "ValidationError",
                "One or more validation errors occurred",
                string.Join("; ", validationErrors));
        }
    }
}