using System.ComponentModel.DataAnnotations;

namespace OneDas.DataManagement.Explorer.Core
{
    public class IsValidNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var name = value as string;

            if (OneDasUtilities.CheckNamingConvention(name, out var errorDescription))
                return ValidationResult.Success;
            else
                return new ValidationResult(errorDescription);
        }
    }
}
