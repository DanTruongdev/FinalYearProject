using OnlineShopping.Models.Customer;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShopping.Models.Purchase
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        [Required]
        public string CustomerId { get; set; }
        [Required]
        public int PaymentId { get; set; }
        public int? UsedPoint { get; set; } = 0;
        public string DeliveryAddress { get; set; }
        public string? Note { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 2,
           ErrorMessage = "Status cannot be less than 2 characters or exceed 20 characters")]
        public string Status { get; set; }
        [Required]
        public bool IsPaid { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        public double TotalCost { get; set; }
        //
        public virtual User Customer { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual ICollection<FurnitureOrderDetail> FurnitureOrderDetails { get; set; }
        public virtual ICollection<CustomizeFurnitureOrderDetail> CustomizeFurnitureOrderDetails { get; set; }
        public virtual ICollection<Warranty>? Warranties { get; set; }
        public virtual ICollection<Feedback>? Feedbacks { get; set; }
    }
}
