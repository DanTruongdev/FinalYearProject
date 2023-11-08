using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels
{
    public class PhoneValidation : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string inputPhoneNums = Convert.ToString(value);
            if (inputPhoneNums.Length != 10) return new ValidationResult("Phone number length must be equal 10 characters");
            if (inputPhoneNums.All(char.IsDigit)) return ValidationResult.Success;
            return new ValidationResult("Phone number contains digit characters only");
        }  
    }
}
