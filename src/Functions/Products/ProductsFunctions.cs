using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using Functions.Interfaces;
using Functions.Dtos;
using Functions.Helpers;

namespace Functions.Products
{
    public class ProductsFunctions(IProductService productService, ILogger<ProductsFunctions> logger)
    {
        private readonly IProductService _productService = productService;
        private readonly ILogger<ProductsFunctions> _logger = logger;

        private const string BadRequestError = "BadRequest";
        private const string InternalErrorType = "InternalError";
        private const string InvalidJsonError = "InvalidJson";
        private const string InvalidProductIdError = "InvalidProductId";
        private const string InvalidStoreIdError = "InvalidStoreId";
        private const string ProductNotFoundError = "ProductNotFound";

        [Function("CreateProduct")]
        public async Task<HttpResponseData> CreateProduct(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "products")] HttpRequestData req)
        {
            try
            {
                _logger.LogInformation("Creating new product");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                if (string.IsNullOrWhiteSpace(requestBody))
                {
                    _logger.LogWarning("Create product request received with empty body");
                    return await ResponseHelper.CreateErrorResponse(
                        req, HttpStatusCode.BadRequest, BadRequestError, "Request body is required");
                }

                CreateProductRequestDto? productDto;
                try
                {
                    productDto = JsonSerializer.Deserialize<CreateProductRequestDto>(requestBody, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "Invalid JSON format in create product request");
                    return await ResponseHelper.CreateErrorResponse(
                        req, HttpStatusCode.BadRequest, InvalidJsonError, "Invalid JSON format");
                }

                if (productDto == null)
                {
                    return await ResponseHelper.CreateErrorResponse(
                        req, HttpStatusCode.BadRequest, BadRequestError, "Invalid product data");
                }

                var (isValid, validationErrors) = ValidationHelper.ValidateModel(productDto);
                if (!isValid)
                {
                    _logger.LogWarning("Create product validation failed: {Errors}", string.Join(", ", validationErrors));
                    return await ResponseHelper.CreateValidationErrorResponse(req, validationErrors);
                }

                var product = ProductMapper.ToEntity(productDto);
                var createdProductJson = await _productService.InsertProductProcedureAsync(product);

                _logger.LogInformation("Product created successfully with ID: {ProductId}", product.Id);
                
                var response = req.CreateResponse(HttpStatusCode.Created);
                response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                await response.WriteStringAsync(createdProductJson);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return await ResponseHelper.CreateErrorResponse(
                    req, HttpStatusCode.InternalServerError, InternalErrorType, $"An error occurred while creating the product: {ex.Message}");
            }
        }

        [Function("GetProducts")]
        public async Task<HttpResponseData> GetProducts([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products")] HttpRequestData req)
        {
            try
            {
                var storeId = req.Query["storeId"];

                if (!ValidationHelper.IsValidGuid(storeId, out Guid storeIdValue))
                {
                    _logger.LogWarning("Get products request with invalid storeId: {StoreId}", storeId);
                    return await ResponseHelper.CreateErrorResponse(
                        req, HttpStatusCode.BadRequest, InvalidStoreIdError, "Invalid or missing storeId parameter");
                }

                _logger.LogInformation("Getting products for store: {StoreId}", storeIdValue);

                var jsonResult = await _productService.GetProductsAsync(storeIdValue);

                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                await response.WriteStringAsync(jsonResult);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products for store");
                return await ResponseHelper.CreateErrorResponse(
                    req, HttpStatusCode.InternalServerError, InternalErrorType, $"An error occurred while retrieving products for the store: {ex.Message}");
            }
        }

        [Function("GetProduct")]
        public async Task<HttpResponseData> GetProduct(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products/{id}")] HttpRequestData req, string id)
        {
            try
            {
                if (!ValidationHelper.IsValidGuid(id, out var productId))
                {
                    _logger.LogWarning("Get product request with invalid ID: {ProductId}", id);
                    return await ResponseHelper.CreateErrorResponse(
                        req, HttpStatusCode.BadRequest, InvalidProductIdError, "Invalid product ID format");
                }

                _logger.LogInformation("Getting product: {ProductId}", productId);

                var product = await _productService.GetByIdAsync(productId);

                if (product == null)
                {
                    _logger.LogInformation("Product not found: {ProductId}", productId);
                    return await ResponseHelper.CreateErrorResponse(
                        req, HttpStatusCode.NotFound, ProductNotFoundError, "Product not found");
                }

                var productDto = ProductMapper.ToDto(product);
                return await ResponseHelper.CreateJsonResponse(req, productDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product: {ProductId}", id);
                return await ResponseHelper.CreateErrorResponse(
                    req, HttpStatusCode.InternalServerError, InternalErrorType, $"An error occurred while retrieving the product: {ex.Message}");
            }
        }

        [Function("UpdateProduct")]
        public async Task<HttpResponseData> UpdateProduct(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "products/{id}")] HttpRequestData req, string id)
        {
            try
            {
                if (!ValidationHelper.IsValidGuid(id, out var productId))
                {
                    _logger.LogWarning("Update product request with invalid ID: {ProductId}", id);
                    return await ResponseHelper.CreateErrorResponse(
                        req, HttpStatusCode.BadRequest, InvalidProductIdError, "Invalid product ID format");
                }

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                if (string.IsNullOrWhiteSpace(requestBody))
                {
                    _logger.LogWarning("Update product request received with empty body for ID: {ProductId}", productId);
                    return await ResponseHelper.CreateErrorResponse(
                        req, HttpStatusCode.BadRequest, BadRequestError, "Request body is required");
                }

                UpdateProductRequestDto? productDto;
                try
                {
                    productDto = JsonSerializer.Deserialize<UpdateProductRequestDto>(requestBody, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "Invalid JSON format in update product request for ID: {ProductId}", productId);
                    return await ResponseHelper.CreateErrorResponse(
                        req, HttpStatusCode.BadRequest, InvalidJsonError, "Invalid JSON format");
                }

                if (productDto == null)
                {
                    return await ResponseHelper.CreateErrorResponse(
                        req, HttpStatusCode.BadRequest, BadRequestError, "Invalid product data");
                }

                var (isValid, validationErrors) = ValidationHelper.ValidateModel(productDto);
                if (!isValid)
                {
                    _logger.LogWarning("Update product validation failed for ID: {ProductId}, Errors: {Errors}", 
                        productId, string.Join(", ", validationErrors));
                    return await ResponseHelper.CreateValidationErrorResponse(req, validationErrors);
                }

                var product = ProductMapper.ToEntity(productDto, productId);
                await _productService.UpdateAsync(product);

                _logger.LogInformation("Product updated successfully: {ProductId}", productId);

                var responseDto = ProductMapper.ToDto(product);
                return await ResponseHelper.CreateJsonResponse(req, responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product: {ProductId}", id);
                return await ResponseHelper.CreateErrorResponse(
                    req, HttpStatusCode.InternalServerError, InternalErrorType, $"An error occurred while updating the product: {ex.Message}");
            }
        }

        [Function("DeleteProduct")]
        public async Task<HttpResponseData> DeleteProduct(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "products/{id}")] HttpRequestData req, string id)
        {
            try
            {
                if (!ValidationHelper.IsValidGuid(id, out var productId))
                {
                    _logger.LogWarning("Delete product request with invalid ID: {ProductId}", id);
                    return await ResponseHelper.CreateErrorResponse(
                        req, HttpStatusCode.BadRequest, InvalidProductIdError, "Invalid product ID format");
                }

                _logger.LogInformation("Deleting product: {ProductId}", productId);

                await _productService.DeleteByIdAsync(productId);

                _logger.LogInformation("Product deleted successfully: {ProductId}", productId);

                return req.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product: {ProductId}", id);
                return await ResponseHelper.CreateErrorResponse(
                    req, HttpStatusCode.InternalServerError, InternalErrorType, $"An error occurred while deleting the product: {ex.Message}");
            }
        }
    }
}