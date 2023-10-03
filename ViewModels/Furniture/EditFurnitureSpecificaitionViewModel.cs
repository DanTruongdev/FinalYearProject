using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels.Furniture
{
    public class EditFurnitureSpecificationViewModel
    {
        [Required]
        [StringLength(39, ErrorMessage = "Furniture specification id must include 39 characters")]
        public string? FurnitureSpecificationId { get; set; }
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "Furniture specification name cannot be less than 2 characters or exceed 50 characters")]
        public string? FurnitureSpecificationName { get; set; }
        public double? Height { get; set; }
        [Range(1, double.MaxValue, ErrorMessage = "Width must be greater than 0")]
        public double? Width { get; set; }
        [Range(1, double.MaxValue, ErrorMessage = "Length must be greater than 0")]
        public double? Length { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "ColorId must be greater than 0")]
        public int? ColorId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "WoodId must be greater than 0")]
        public int? WoodId { get; set; }
        [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public double? Price { get; set; }
        [StringLength(300, MinimumLength = 2,
            ErrorMessage = "Description cannot be less than 2 characters or exceed 300 characters")]
        public string? Description { get; set; }
        public List<IFormFile> UploadFiles { get; set; }
        
    }
}
