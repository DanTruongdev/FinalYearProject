using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels.Furniture
{
    public class EditFurnitureViewModel
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "FurnitureId must be greater than 0")]
        public int FurnitureId { get; set; }
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "Furniture name cannot be less than 2 characters or exceed 50 characters")]
        public string? FurnitureName { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be greater than 0")]
        public int? CategoryId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "LabelId must be greater than 0")]
        public int? LabelId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "CollectionId must be greater than 0")]
        public int? CollectionId { get; set; }
        [StringLength(50, MinimumLength = 2,
           ErrorMessage = "Appropriate room cannot be less than 2 characters or exceed 50 characters")]
        public string? AppropriateRoom { get; set; }
       
    }
}
