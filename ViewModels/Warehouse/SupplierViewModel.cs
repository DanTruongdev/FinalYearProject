using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels.Warehouse
{
    public class SupplierViewModel
    {
        [Required]
        [StringLength(30, MinimumLength = 2,
          ErrorMessage = "Suplier name cannot be less than 2 characters or exceed 30 characters")]
        public string SupplierName { get; set; }
        public IFormFile? SuplierImage { get; set; }
        [Required]
        [EmailAddress]
        public string? SuplierEmail { get; set; }
        [Required]
        [PhoneValidation]
        public string SuplierPhoneNums { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 2,
          ErrorMessage = "Street cannot be less than 2 characters or exceed 30 characters")]
        public string Street { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 2,
          ErrorMessage = "Ward cannot be less than 2 characters or exceed 30 characters")]
        public string Ward { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 2,
          ErrorMessage = "District cannot be less than 2 characters or exceed 20 characters")]
        public string District { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 2,
          ErrorMessage = "Provine cannot be less than 2 characters or exceed 20 characters")]
        public string Provine { get; set; }

    }
}
