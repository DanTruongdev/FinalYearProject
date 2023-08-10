using OnlineShopping.Models.Funiture;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.Purchase
{
    public class WishList
    {
        [Key]
        public int WishlistId { get; set; }
        [Required]
        public string CustomerId { get; set; }
        //
        public User Customer { get; set; }
        public ICollection<WishListDetail> WishListDetails { get; set;}
      
    }
}
