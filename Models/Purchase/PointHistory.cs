using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.Purchase
{
    public class PointHistory
    {
        [Key]
        public int PointHistoryId { get; set; }
        [Required]
        public string CustomerId { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2,
          ErrorMessage = "Description cannot be less than 2 characters or exceed 50 characters")]
        public string Description { get; set; }
        public DateTime History { get; set; }
        //
        public virtual User User { get; set; }
    }
}
