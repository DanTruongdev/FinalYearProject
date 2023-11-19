using OnlineShopping.Models.Gallery;
using OnlineShopping.Models.Purchase;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.Funiture
{
    public class CustomizeFurniture
    {
        [Key]
        public string CustomizeFurnitureId { get; set; }
        [Required]
        public string CustomerId { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "Category cannot be less than 2 characters or exceed 50 characters")]
        public string CustomizeFurnitureName { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int ColorId { get; set; }
        [Required]
        public double Height { get; set; }
        [Required]
        public double Width { get; set; }
        [Required]
        public double Length { get; set; }
        [Required]
        public int WoodId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [StringLength(500, MinimumLength = 2,
            ErrorMessage = "Description cannot be less than 2 characters or exceed 500 characters")]
        public string? Description { get; set; }
        public DateTime DesiredCompletionDate { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }

        //
        public virtual Category Category { get; set; }
        public virtual ICollection<CustomizeFurnitureAttachment> Attachments { get; set; }
        public virtual Color Color { get; set; }
        public virtual Wood Wood { get; set; }
        public virtual User Customer { get; set; }
        public virtual ICollection<CustomizeFurnitureOrderDetail> CustomizeFurnitureOrderDetails { get; set; }
        public virtual Result Result { get; set; }
    }
}
