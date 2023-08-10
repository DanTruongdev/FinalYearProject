using OnlineShopping.Models.Funiture;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace OnlineShopping.Models.Customer
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }
        [Required]
        public string CustomerId { get; set; }
        [Required]
        public int FurnitureSpecificationId { get; set; }
        [Required]
        [StringLength(120, MinimumLength = 2,
          ErrorMessage = "Feedback cannot be less than 2 characters or exceed 120 characters")]
        public string Content { get; set; }
        [Required]
        public int VoteStar { get; set; }
        public bool Anonymous { get; set; }
        public DateTime CreationDate { get; set; }
        //
        public User Customer { get; set; }
        public FurnitureSpecification FurnitureSpecification { get; set; }
    }
}
