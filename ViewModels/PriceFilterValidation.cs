using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels
{
    public class PriceFilterValidation : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var model = (ViewModels.Furniture.FurnitureFilterViewModel)validationContext.ObjectInstance;
            try
            {
                double minCost = model.MinCost;
                double maxCost = model.MaxCost;
                if (minCost < 0 || minCost > maxCost) return new ValidationResult("The min cost must be greater than 0 and less than max cost");
            }
            catch (Exception ex)
            {
                new ValidationResult($"Invalid input min cost and max cost");
            }
            return ValidationResult.Success;
        }
    }
}
