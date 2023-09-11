using OnlineShopping.Models.Funiture;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShopping.Models.Purchase
{
    public class CartDetail
    {
        [Key]
        public int CartDetailId { get; set; }
        [Required]
        public int CartId { get; set; }
        [Required]
        public string FurnitureSpecificationId { get; set; }
        public int Quantity { get; set; } 
        [NotMapped]
        public double Cost { get; set; }
        //
        public virtual Cart Cart { get; set; }
        public virtual FurnitureSpecification FurnitureSpecifition { get; set; }
    }
}
