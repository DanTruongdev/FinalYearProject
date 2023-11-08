using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.Funiture
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 2, 
            ErrorMessage = "Category cannot be less than 2 characters or exceed 20 characters")]
        public string CategoryName { get; set; }
        ///
        public virtual ICollection<Furniture> Furnitures { get; set;}
        public virtual ICollection<CustomizeFurniture> CustomizeFurnitures { get; set; }
    }
}
