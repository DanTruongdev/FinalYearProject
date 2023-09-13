using OnlineShopping.Models.Customer;
using OnlineShopping.Models.Funiture;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.Gallery
{
    public class FurnitureSpecificationAttachment
    {
        [Key]
        public int FurnitureSpecificationAttachemnetId { get; set; }
        [Required]
        public string FurnitureSpecificationId { get; set; }
        [Required]
        public string AttachmentName { get; set; }
        [Required]
        public string Path { get; set; }
        [Required]
        public string Type { get; set; } //video or image...
        //
        public virtual FurnitureSpecification FurnitureSpecification { get; set; }
    }
}
