using System.ComponentModel.DataAnnotations;

namespace Functions.Dtos.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class RequiredNotEmptyAttribute : ValidationAttribute
    {
        public RequiredNotEmptyAttribute() : base("The {0} field is required and cannot be empty or whitespace.")
        {
        }

        public override bool IsValid(object? value)
        {
            if (value is string stringValue)
            {
                return !string.IsNullOrWhiteSpace(stringValue);
            }
            return value != null;
        }
    }
}