using OnlineShopping.Models.Gallary;
using OnlineShopping.Models.Purchase;
using System.ComponentModel.DataAnnotations;


namespace OnlineShopping.Models.Customer
{
    public class WarrantySchedule
    {
        [Key]
        public int WarrantyScheduleId { get; set; }
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
            ErrorMessage ="Status cannot be less than 2 characters or exceed 20 characters")]
        public string Status { get; set; }
        //
        public User User { get; set; }
        public Order Order { get; set; }
        public ICollection<Attachment> Attachments { get; set; }


    }
}
