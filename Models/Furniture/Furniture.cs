using OnlineShopping.Models.Purchase;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShopping.Models.Funiture
{
    public class Furniture
    {
        [Key]
        public int FurnitureId { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "Furniture name cannot be less than 2 characters or exceed 50 characters")]
        public string FurnitureName { get; set; }
        public int? CategoryId { get; set; }
        public int? LabelId { get; set; }
        public int? CollectionId { get; set; }
        public string AppopriateRoom { get; set; }
        public int Like { get; set; } = 0;
        public int Sold { get; set; } = 0;
        public double VoteStar { get; set; }
        //
        public virtual Category? Category { get; set; }
        public virtual  Label? Label { get; set; }
        public virtual Collection? Collection { get; set; }
<<<<<<<< HEAD:Models/Furniture/Furniture.cs

========
>>>>>>>> fcb14cdb0cf9aa6a680a0a059cc55b426b15b924:Models/Funiture/Furniture.cs
        public virtual ICollection<FurnitureSpecification> FurnitureSpecifications { get; set; }           
        public virtual ICollection<WishListDetail>? WishListDetails { get; set; }
      
    }
}
