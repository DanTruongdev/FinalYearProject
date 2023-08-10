using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.Funiture
{
    public class Wood
    {
        [Key]
        public int WoodId { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 2,
            ErrorMessage = "Wood type cannot be less than 2 characters or exceed 30 characters")]
        public string WoodType { get; set; }
        //
        public ICollection<FurnitureSpecification> FurnitureSpecification { get; set; }
        public ICollection<Requirement> Requirements { get; set; }

    }
}
