using OnlineShopping.Models.Customer;
using OnlineShopping.Models.Funiture;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.Gallery
{
    public class FeedbackAttachment
    {
        [Key]
        public int FeedbackAttachmentId { get; set; }
        [Required]
        public int FeedbackId { get; set; }
        [Required]
        public string AttachmentName { get; set; }
        [Required]
        public string Path { get; set; }
        [Required]
        public string Type { get; set; } //video or image...
        //
        public virtual Feedback Feedback { get; set; }
    }
}
