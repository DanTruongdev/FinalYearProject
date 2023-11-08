using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels.Warranty
{
    public class WarrantyViewModel
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 2,
            ErrorMessage = "Status cannot be less than 2 characters or exceed 20 characters")]
        public string WarrantyReasons { get; set; }
        [Required]
        [FileValidation]
        public List<IFormFile> UploadFiles { get; set; }
    }
}
