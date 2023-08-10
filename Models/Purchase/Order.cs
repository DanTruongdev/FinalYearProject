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
 
        public int? PaymentId { get; set; }
        public int? UsedPoint { get; set; } = 0;
        [NotMapped]
        public string Address { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 2,
           ErrorMessage = "Status cannot be less than 2 characters or exceed 20 characters")]
        public string Status { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [NotMapped]
        public double TotalCost { get; set; }
        //
        public User Customer { get; set; }
        public Payment? Payment { get; set; }
        public ICollection<FurnitureOrderDetail> FurnitureOrderDetails { get; set; }
        public ICollection<CustomizeFurnitureOrderDetail> CustomizeFurnitureOrderDetails { get; set; }
        public ICollection<WarrantySchedule>? WarrantySchedules { get; set; }
    }
}
