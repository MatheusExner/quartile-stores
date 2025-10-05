using Domain.Entities;

namespace Functions.Interfaces;

public interface IProductService
{
    /// <summary>
    /// Get products as a JSON string for a specific store
    /// </summary>
    /// <param name="storeId"></param>
    /// <returns></returns>
    Task<string> GetProductsAsync(Guid storeId);

    /// <summary>
    /// Insert a new product using a stored procedure
    /// </summary>
    /// <param name="product"></param>
    /// <returns></returns>
    Task<string> InsertProductProcedureAsync(Product product);

    /// <summary>
    /// Get a product by its unique identifier
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Product?> GetByIdAsync(Guid id);

    /// <summary>
    /// Update an existing product
    /// </summary>
    /// <param name="product"></param>
    /// <returns></returns>
    Task UpdateAsync(Product product);

    /// <summary>
    /// Delete a product by its unique identifier
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteByIdAsync(Guid id);
}