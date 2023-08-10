using OnlineShopping.Models.Customer;
using OnlineShopping.Models.Funiture;
using OnlineShopping.Models.Warehouse;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.Gallary
{
    public class Attachment
    {
        [Key]
        public int AttachmentId { get; set; }
        [Required] 
        public string FileName { get; set; }
        [Required]
        public int LikedItemId { get; set; }
        [Required]
        public string AttachmentFor { get; set; }
        public string? Path { get; set; }     
        //
        public FurnitureSpecification? FurnitureSpecification { get; set; }
        public Requirement? Requirement { get; set; }
        public Material? Material { get; set; }
        public Feedback? Feedback { get; set; }
        public Suplier? Suplier { get; set; }  
        public WarrantySchedule? WarrantySchedule { get; set; }

    }
}
