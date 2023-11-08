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
        public string CustomizeFunitureId { get; set; }
        [Required]
        [Range(0, 50, ErrorMessage = "Quantity must be in range 1-50")]
        public int Quantity { get; set; }
        [Required]
        [Range(0,double.MaxValue, ErrorMessage = "Cost must be greater than 0")]
        public double Cost { get; set; }
        //
        public virtual Order Order { get; set; }
        public virtual CustomizeFurniture CustomizeFurniture { get; set; }

    }
}
