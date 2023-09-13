using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels.Furniture
{
    public class ResultViewModel
    {
        
        [Required]
        public string CustomizeFurnitureId { get; set; }
        [Required]
        [DateTimeValidation(ErrorMessage = "Actual completion date cannot be less than current date")]
        public DateTime ActualCompletionDate { get; set; }
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Expected price must be greater than 1")]
        public double ExpectedPrice { get; set; }
        [Required]
        [CustomizeFurnitureStatus]
        public string Status { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Reason cannot be less than 2 characters or exceed 150 characters")]
        public string Reason { get; set; }
    }
    
    public class CustomizeFurnitureStatus : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            string input = value.ToString();
            if (!input.Equals("Accepted") && !input.Equals("Not accepted")) return new ValidationResult("Status must be \"Accepted\" or \"Not accepted\"");   
            return ValidationResult.Success;
        }
    } 
}
