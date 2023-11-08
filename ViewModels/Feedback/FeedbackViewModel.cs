using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels.Feedback
{
    public class FeedbackViewModel
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        public string FurnitureSpecificationId { get; set; }
        [StringLength(120, MinimumLength = 2,
          ErrorMessage = "Feedback cannot be less than 2 characters or exceed 120 characters")]
        public string Content { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Vote star must be between 1 and 5.")]
        public int VoteStar { get; set; }
        [Required]
        public bool Anonymous { get; set; }
        [FileValidation]
        public List<IFormFile> files { get; set; } = new List<IFormFile>();
       
    }
}
