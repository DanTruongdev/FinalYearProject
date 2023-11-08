using OnlineShopping.Models.Warehouse;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels.Warehouse
{
    public class MaterialViewModel
    {
        [Required]
        [StringLength(30, MinimumLength = 2,
           ErrorMessage = "Material name cannot be less than 2 characters or exceed 30 characters")]
        public string MaterialName { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Material price must be greater than 0")]
        public int MaterialPrice { get; set; }
        [Required]
        public IFormFile UploadImage { get; set; }
        [StringLength(50, MinimumLength = 2,
           ErrorMessage = "Description  cannot be less than 2 characters or exceed 50 characters")]
        public string? Description { get; set; }
        [Required]
        public int DefaultSuplierId { get; set; }
       
    }
}
