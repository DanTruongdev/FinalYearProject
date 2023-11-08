using OnlineShopping.Models.Customer;
using OnlineShopping.Models.Funiture;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.Gallery
{
    public class CustomizeFurnitureAttachment
    {
        [Key]
        public int CustomizeFurnitureAttachmentId { get; set; }
        [Required]
        public string CustomizeFurnitureId { get; set; }
        [Required]
        public string AttachmentName { get; set; }
        [Required]
        public string Path { get; set; }
        [Required]
        public string Type { get; set; } //video or image...
        //
        public virtual CustomizeFurniture CustomizeFurniture { get; set; }
    }
}
