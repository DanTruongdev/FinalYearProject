using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels.Warranty
{
    public class EditWarrantyViewModel
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Warranty Id must be grater than 0")]
        public int WarrantyId { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 2,
            ErrorMessage = "Status cannot be less than 2 characters or exceed 20 characters")]
        public string? WarrantyReasons { get; set; }
        
        [FileValidation]
        public List<IFormFile>? UploadFiles { get; set; }
    }
}
