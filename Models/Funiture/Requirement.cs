using OnlineShopping.Models.Gallary;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.Funiture
{
    public class Requirement
    {
        [Key]
        public int RequirementId { get; set; }
        [Required]
        public int CustomizeFurnitureId { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "Category cannot be less than 2 characters or exceed 50 characters")]
        public string CustomizeFurnitureName { get; set; }
        public int? CategoryId { get; set; }   
        public int? ColorId { get; set; }
        [Required]
        public double Height { get; set; }
        [Required]
        public double Width { get; set; }
        [Required]
        public double Length { get; set; }
        public int? WoodId { get; set; }
        [Required]
        public int Quantity { get; set; }
        public DateTime DesiredCompletionDate { get; set; }
        public string? Description { get; set; }
        //
        public Category? Category { get; set; }
        public ICollection<Attachment> Attachments { get; set; }
        public Color? Color { get; set; }
        public Wood? Wood { get; set; }
        public CustomizeFurniture CustomizeFurniture { get; set; }
    }
}
