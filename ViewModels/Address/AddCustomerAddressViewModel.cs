using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace OnlineShopping.ViewModels.Address
{
    public class AddCustomerAddressViewModel 
    {
        [Required]
        [StringLength(50, MinimumLength = 2,
          ErrorMessage = "Street cannot be less than 2 characters or exceed 50 characters")]
        public string Street { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2,
          ErrorMessage = "Commune cannot be less than 2 characters or exceed 50 characters")]
        public string Commune { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2,
          ErrorMessage = "District cannot be less than 2 characters or exceed 50 characters")]
        public string District { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2,
          ErrorMessage = "Provine cannot be less than 2 characters or exceed 50 characters")]
        public string Provine { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2,
          ErrorMessage = "Address type cannot be less than 2 characters or exceed 20 characters")]
        public string Type { get; set; }
    }
}
