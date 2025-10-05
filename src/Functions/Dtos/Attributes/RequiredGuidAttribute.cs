using System.ComponentModel.DataAnnotations;

namespace Functions.Dtos.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class RequiredGuidAttribute : ValidationAttribute
    {
        public RequiredGuidAttribute() : base("The {0} field is required and cannot be empty.")
        {
        }

        public override bool IsValid(object? value)
        {
            if (value is Guid guid)
            {
                return guid != Guid.Empty;
            }
            return false;
        }
    }
}