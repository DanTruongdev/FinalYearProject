using Castle.Core.Internal;
using OnlineShopping.Libraries.Services;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels
{
    public class GenderValidation : ValidationAttribute
    {
        private readonly List<string> ValidGender = new List<string>() {"MALE", "FEMALE", "OTHER"};
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            
            string userInput = Convert.ToString(value).ToUpper();    
            if (!userInput.IsNullOrEmpty())
            {
                if (!ValidGender.Contains(userInput)) return new ValidationResult("The gender must be "+"Male"+", "+"Female"+" or "+"Other");

            }
            return ValidationResult.Success;
        }
    }
}
