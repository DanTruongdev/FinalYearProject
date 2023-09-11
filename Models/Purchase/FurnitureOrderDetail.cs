using OnlineShopping.Models.Funiture;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShopping.Models.Purchase
{
    public class FurnitureOrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; }
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int FurnitureSpecificationId { get; set; }
        public int Quantity { get; set; } = 1;
        public double Cost { get; set; }
        //
        public virtual Order Order { get; set; }
        public virtual FurnitureSpecification FurnitureSpecification { get; set; }
    }
}
