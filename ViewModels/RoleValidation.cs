using OnlineShopping.Libraries.Services;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels
{
    public class RoleValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            string role = value as string;
            role = role.ToUpper();
            if (role.Equals("CUSTOMER") && role.Equals("ASSISTANT") && role.Equals("SHOP OWNER") && role.Equals("ADMIN")) 
                return new ValidationResult("Role must be \"CUSTOMER\", \"ASSISTANT\", \"SHOP OWNER\", \"ADMIN\"");
            return ValidationResult.Success;
        }
    }
}
