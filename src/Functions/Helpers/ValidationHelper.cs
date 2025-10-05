using System.ComponentModel.DataAnnotations;

namespace Functions.Helpers
{
    public static class ValidationHelper
    {
        public static (bool IsValid, List<string> Errors) ValidateModel<T>(T model) where T : class
        {
            if (model == null)
            {
                return (false, new List<string> { "Model cannot be null" });
            }

            var validationContext = new ValidationContext(model, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            
            // Validate all properties including data annotations
            bool isValid = Validator.TryValidateObject(model, validationContext, validationResults, validateAllProperties: true);
            
            var errors = validationResults.Select(vr => vr.ErrorMessage ?? "Unknown validation error").ToList();
            
            return (isValid, errors);
        }

        public static bool IsValidGuid(string? guidString, out Guid guid)
        {
            return Guid.TryParse(guidString, out guid) && guid != Guid.Empty;
        }
    }
}