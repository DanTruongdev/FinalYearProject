using OnlineShopping.Models.Funiture;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShopping.Models.Purchase
{
    public class CustomizeFurnitureOrderDetail
    {
        [Key]
        public int CustomizeFurnitureOrderDetailId { get; set; }
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int CustomizeFunitureId { get; set; }
        [NotMapped]
        public double Cost { get; set; }
        //
        public Order Order { get; set; }
        public CustomizeFurniture CustomizeFurniture { get; set; }

    }
}
