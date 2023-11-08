using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels
{
    public class DobValidation : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
           DateTime inputDateTime = Convert.ToDateTime(value);
            if (inputDateTime >  DateTime.Now) 
            {
                return new ValidationResult(base.ErrorMessage);
                   
            } return ValidationResult.Success;
        }
    }
}
