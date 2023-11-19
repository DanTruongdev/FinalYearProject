using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace OnlineShopping.ViewModels.Warehouse
{
    public class ImportViewModel<T>
    {
        [Required]
        [StringLength(300, MinimumLength = 2, ErrorMessage = "Export reason cannot be less than 2 characters or exceed 300 characters")]
        public string ImportReason { get; set; }
        [Required]
        [DobValidation]
        public DateTime ImportDate { get; set; }
        [Required]
        public IList<ItemViewModel<T>> Items { get; set; }
    } 
}
