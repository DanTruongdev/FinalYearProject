using OnlineShopping.Models.Gallery;
using OnlineShopping.Models.Purchase;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.Customer
{
    public class Warranty
    {
        [Key]
        public int WarrantyId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int OrderId { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 2,
            ErrorMessage = "Status cannot be less than 2 characters or exceed 20 characters")]
        public string WarrantyReasons { get; set; }
        public DateTime? EstimatedTime { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 2,
            ErrorMessage = "Status cannot be less than 2 characters or exceed 20 characters")]
        public string Status { get; set; }
        //
        public virtual User User { get; set; }
        public virtual Order Order { get; set; }
        public virtual ICollection<WarrantyAttachment> Attachments { get; set; }
    }
}
