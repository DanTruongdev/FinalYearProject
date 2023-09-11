using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels.Furniture
{
    public class CustomizeFurnitureViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "Category cannot be less than 2 characters or exceed 50 characters")]
        public string CustomizeFurnitureName { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int ColorId { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Height must be greater than 0")]
        public double Height { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Width must be greater than 0")]
        public double Width { get; set; }
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Length must be greater than 0")]                 
        public double Length { get; set; }
        [Required]
        public int WoodId { get; set; }
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
        [Required]
        [DateTimeValidation(ErrorMessage = "Desired completion date must be greater than current time")]
        public DateTime DesiredCompletionDate { get; set; }
        [StringLength(500, MinimumLength = 2,
            ErrorMessage = "Description cannot be less than 2 characters or exceed 500 characters")]
        public string? Description { get; set; }
        [FileValidation]
        public List<IFormFile>? Attachments { get; set; }
    }
}
