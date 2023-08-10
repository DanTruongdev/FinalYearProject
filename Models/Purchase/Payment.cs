using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.Purchase
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 2,
          ErrorMessage = "Payment method cannot be less than 2 characters or exceed 30 characters")]
        public string PaymentMethod { get; set; }
        //
        public ICollection<Order> Orders { get; set; }
    }
}
