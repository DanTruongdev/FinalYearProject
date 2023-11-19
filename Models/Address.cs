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
        public string AddressOwner { get; set; }
        public override string ToString()
        {
            return $"{Street}, {Ward}, {District}, {Provine}";
        }
        //
        public virtual Supplier? Supplier { get; set; }   
        public virtual Repository? Repository { get; set; }
        public virtual ICollection<UserAddress>? UserAddresses { get; set; }
     
    }
}
