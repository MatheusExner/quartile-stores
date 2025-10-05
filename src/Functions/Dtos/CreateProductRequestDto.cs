using System.ComponentModel.DataAnnotations;
using Functions.Dtos.Attributes;

namespace Functions.Dtos
{
    public class CreateProductRequestDto
    {
        [RequiredNotEmpty(ErrorMessage = "Product name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Product name must be between 1 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [RequiredGuid(ErrorMessage = "Store ID is required")]
        public Guid StoreId { get; set; }
    }
}