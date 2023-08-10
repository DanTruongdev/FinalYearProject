using OnlineShopping.Models.Customer;
using OnlineShopping.Models.Purchase;
using OnlineShopping.Models.Warehouse;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 2,
          ErrorMessage = "Address cannot be less than 2 characters or exceed 30 characters")]
        public string Street { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 2,
          ErrorMessage = "Address cannot be less than 2 characters or exceed 30 characters")]
        public string Commune { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 2,
          ErrorMessage = "Address cannot be less than 2 characters or exceed 20 characters")]
        public string District { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 2,
          ErrorMessage = "Address cannot be less than 2 characters or exceed 20 characters")]
        public string Provine { get; set; }
        public string AddressOwner { get; set; }
        //
        public Suplier? Suplier { get; set; }   
        public Repository? Repository { get; set; }
        public ICollection<UserAddress>? UserAddresses { get; set; }
     
    }
}
