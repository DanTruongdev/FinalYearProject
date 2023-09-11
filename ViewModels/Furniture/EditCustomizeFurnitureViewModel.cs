using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels.Furniture
{
    public class EditCustomizeFurnitureViewModel
    {
        [Required]
        public int CustomizeFurnitureId { get; set; }
      
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "Category cannot be less than 2 characters or exceed 50 characters")]
        public string? CustomizeFurnitureName { get; set; }
        public int? CategoryId { get; set; }
        public int? ColorId { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Height must be greater than 0")]
        public double? Height { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Width must be greater than 0")]
        public double? Width { get; set; }
        [Range(1, double.MaxValue, ErrorMessage = "Length must be greater than 0")]                 
        public double? Length { get; set; }
        public int? WoodId { get; set; }
        [Range(1, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int? Quantity { get; set; }
        [DateTimeValidation(ErrorMessage = "Desired completion date must be greater than current time")]
        public DateTime? DesiredCompletionDate { get; set; }
        [StringLength(500, MinimumLength = 2,
            ErrorMessage = "Description cannot be less than 2 characters or exceed 500 characters")]
        public string? Description { get; set; }
        [FileValidation]
        public List<IFormFile>? Attachments { get; set; }
    }
}
