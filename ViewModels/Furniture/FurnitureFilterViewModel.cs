using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace OnlineShopping.ViewModels.Furniture
{
    public class FurnitureFilterViewModel
    {
        [PriceFilterValidation]
        public double MinCost { get; set; }
        [PriceFilterValidation()]
        public double MaxCost { get; set; }
        [StringLength(40, MinimumLength = 2,
          ErrorMessage = "Category cannot be less than 2 characters or exceed 40 characters")]
        public string? Category { get; set; }
        [Range(0,5,ErrorMessage = "The vote star must be between 1 and 5")]
        public int Star { get; set; }
        [StringLength(120, MinimumLength = 2,
          ErrorMessage = "AppropriateRoom cannot be less than 2 characters or exceed 120 characters")]
        public string? AppropriateRoom { get; set; }
        [StringLength(50, MinimumLength = 2,
          ErrorMessage = "Collection cannot be less than 2 characters or exceed 50 characters")]
        public string? Collection { get; set; }
    }
}
