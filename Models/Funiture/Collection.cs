using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.Funiture
{
    public class Collection
    {
        [Key]
        public int CollectionId { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2,
           ErrorMessage = "Collection name cannot be less than 2 characters or exceed 50 characters")]
        public string CollectionName { get; set; }
        //
        public ICollection<Furniture> Furnitures { get; set;}
    }
}
