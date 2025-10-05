using System.Data;
using Dapper;
using Domain.Entities;
using Functions.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Functions.Services
{
    public class ProductService(IConfiguration configuration, ILogger<ProductService> logger) : IProductService
    {
        private readonly string _connectionString = configuration.GetConnectionString("SQLServer")
                     ?? configuration["SQLServer"]
                     ?? throw new InvalidOperationException("Missing SqlConnectionString in configuration.");
        private readonly ILogger<ProductService> _logger = logger;

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        /// <summary>
        /// Get products as a JSON string for a specific store
        /// </summary>
        /// <param name="storeId">Store's Id</param>
        /// <returns></returns>
        public async Task<string> GetProductsAsync(Guid storeId)
        {
            try
            {
                _logger.LogInformation("Getting products for store: {StoreId}", storeId);

                using var conn = CreateConnection();
                
                const string sql = "SELECT dbo.fn_GetProductsJson(@StoreId)";
                var parameters = new { StoreId = storeId };

                var json = await conn.ExecuteScalarAsync<string>(sql, parameters);
                
                _logger.LogInformation("Retrieved {Count} products for store: {StoreId}", 
                    json?.Length > 2 ? "multiple" : "no", storeId);
                
                return json ?? "[]";
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error getting products for store: {StoreId}", storeId);
                throw new InvalidOperationException($"Database error occurred while retrieving products {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error getting products for store: {storeId} {ex.Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Insert a new product using a stored procedure
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task<string> InsertProductProcedureAsync(Product product)
        {
            try
            {
                _logger.LogInformation("Inserting new product: {ProductName} for store: {StoreId}", 
                    product.Name, product.StoreId);

                using var conn = CreateConnection();
                
                const string proc = "dbo.InsertProduct";
                var parameters = new
                {
                    product.StoreId,
                    product.Name,
                    product.Description,
                    product.Price
                };

                var json = await conn.ExecuteScalarAsync<string>(
                    proc,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                _logger.LogInformation("Successfully inserted product: {ProductName}", product.Name);
                return json ?? "{}";
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error inserting product: {ProductName}", product.Name);
                throw new InvalidOperationException($"Database error occurred while inserting product {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error inserting product: {ProductName}", product.Name);
                throw new InvalidOperationException($"An unexpected error occurred while inserting product {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get a product by its ID
        /// </summary>
        /// <param name="id">Product's id</param>
        /// <returns></returns>
        public async Task<Product?> GetByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Getting product by ID: {ProductId}", id);

                using var conn = CreateConnection();

                const string query = @"
                    SELECT Id, Name, Description, Price, StoreId 
                    FROM Products 
                    WHERE Id = @Id";

                var product = await conn.QueryFirstOrDefaultAsync<Product>(query, new { Id = id });
                
                if (product == null)
                {
                    _logger.LogInformation("Product not found: {ProductId}", id);
                }
                else
                {
                    _logger.LogInformation("Found product: {ProductId}", id);
                }

                return product;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error getting product by ID: {ProductId}", id);
                throw new InvalidOperationException($"Database error occurred while retrieving product {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error getting product by ID: {ProductId}", id);
                throw new InvalidOperationException($"An unexpected error occurred while retrieving product {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task UpdateAsync(Product product)
        {
            try
            {
                _logger.LogInformation("Updating product: {ProductId}", product.Id);

                const string sql = @"
                    UPDATE Products
                    SET 
                        Name = @Name,
                        Description = @Description,
                        Price = @Price,
                        StoreId = @StoreId
                    WHERE Id = @Id";

                using var connection = CreateConnection();

                var rowsAffected = await connection.ExecuteAsync(sql, new
                {
                    product.Id,
                    product.Name,
                    product.Description,
                    product.Price,
                    product.StoreId
                });

                if (rowsAffected == 0)
                {
                    _logger.LogWarning("No rows affected when updating product: {ProductId}", product.Id);
                    throw new InvalidOperationException($"Product with ID {product.Id} not found or could not be updated");
                }

                _logger.LogInformation("Successfully updated product: {ProductId}", product.Id);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error updating product: {ProductId}", product.Id);
                throw new InvalidOperationException($"Database error occurred while updating product {ex.Message}", ex);
            }
            catch (Exception ex) when (!(ex is InvalidOperationException))
            {
                _logger.LogError(ex, "Unexpected error updating product: {ProductId}", product.Id);
                throw new InvalidOperationException($"An unexpected error occurred while updating product {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Delete a product by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting product: {ProductId}", id);

                const string sql = "DELETE FROM Products WHERE Id = @Id";
                using var connection = CreateConnection();
                
                var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
                
                if (rowsAffected == 0)
                {
                    _logger.LogWarning("No rows affected when deleting product: {ProductId}", id);
                    throw new InvalidOperationException($"Product with ID {id} not found or could not be deleted");
                }

                _logger.LogInformation("Successfully deleted product: {ProductId}", id);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error deleting product: {ProductId}", id);
                throw new InvalidOperationException($"Database error occurred while deleting product {ex.Message}", ex);
            }
            catch (Exception ex) when (!(ex is InvalidOperationException))
            {
                _logger.LogError(ex, "Unexpected error deleting product: {ProductId}", id);
                throw new InvalidOperationException($"An unexpected error occurred while deleting product {ex.Message}", ex);
            }
        }

    }
}