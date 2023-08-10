using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.Funiture
{
    public class Label
    {
        [Key]
        public int LabelId { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 2,
            ErrorMessage = "Label cannot be less than 2 characters or exceed 20 characters")]
        public string LabelName { get; set; }
        //
        public ICollection<Furniture> Furnitures { get; set; }
    }
}
