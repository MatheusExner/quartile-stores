using Domain.Entities;
using Functions.Dtos;

namespace Functions.Helpers
{
    public static class ProductMapper
    {
        public static Product ToEntity(CreateProductRequestDto dto)
        {
            return new Product
            {
                Id = Guid.NewGuid(),
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim() ?? string.Empty,
                Price = dto.Price,
                StoreId = dto.StoreId
            };
        }

        public static Product ToEntity(UpdateProductRequestDto dto, Guid id)
        {
            return new Product
            {
                Id = id,
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim() ?? string.Empty,
                Price = dto.Price,
                StoreId = dto.StoreId
            };
        }

        public static ProductResponseDto ToDto(Product entity)
        {
            return new ProductResponseDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                StoreId = entity.StoreId
            };
        }
    }
}