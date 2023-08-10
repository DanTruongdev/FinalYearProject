using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.Purchase
{
    public class Point
    {
        [Key]
        public int PointId { get; set; }
        [Required]
        public string CustomerId { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2,
          ErrorMessage = "Description cannot be less than 2 characters or exceed 50 characters")]
        public string Description { get; set; }
        public int TotalPoint { get; set; } = 0;
        public DateTime History { get; set; }
        //
        public User User { get; set; }
    }
}
