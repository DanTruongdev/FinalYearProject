using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels
{
    public class DateTimeValidation : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;
            DateTime inputDateTime = Convert.ToDateTime(value);
            if (inputDateTime < DateTime.Now)
            {
                return new ValidationResult(base.ErrorMessage);

            }
            return ValidationResult.Success;
        }
    }
}
