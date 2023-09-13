using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.Funiture
{
    public class Color
    {
        [Key]
        public int ColorId { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 3, 
            ErrorMessage = "Color name  cannot be less than 3 characters or exceed 20 characters")]
        public string ColorName { get; set; }
        //
        public virtual ICollection<FurnitureSpecification> FurnitureSpecifications { get; set; }
        public virtual ICollection<CustomizeFurniture> CustomizeFurnitures { get; set; }

    }
}
