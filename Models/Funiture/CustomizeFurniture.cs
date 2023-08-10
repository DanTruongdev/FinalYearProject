using OnlineShopping.Models.Customer;
using OnlineShopping.Models.Purchase;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.Funiture
{
    public class CustomizeFurniture
    {
        [Key]
        public int CustomizeFurnitureId { get; set; }
        [Required]
        public string CustomerId { get; set; }
        [Required]
        [StringLength(10, MinimumLength = 2,
            ErrorMessage = "Status cannot be less than 2 characters or exceed 10 characters")]
        public double? ExpectedPrice { get; set; }
        public string Status { get; set; }
        //
        public User Customer { get; set; }
        public ICollection<Requirement> Requirements { get; set; }
        public ICollection<CustomizeFurnitureOrderDetail> CustomizeFurnitureOrderDetails { get; set; }
          
    }
}
