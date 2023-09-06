using OnlineShopping.Models.Customer;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.Gallery
{
    public class WarrantyAttachment
    {
        [Key]
        public int AttachmentId { get; set; }
        [Required]
        public int WarrantyId { get; set; }
        [Required]
        public string AttachmentName { get; set; }
        [Required]
        public string Path { get; set; }
        [Required]
        public string Type { get; set; } //video or image...
        //
        public virtual Warranty Warranty { get; set; }
    }
}
