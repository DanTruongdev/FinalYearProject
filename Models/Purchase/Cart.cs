using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.Purchase
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }
        [Required]
        public string CustomerId { get; set; }
        //
        public virtual User Customer { get; set; }
        public virtual ICollection<CartDetail> CartDetails { get; set; }
    }
}
