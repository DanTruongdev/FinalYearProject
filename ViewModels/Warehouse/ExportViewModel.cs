using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels.Warehouse
{
    public class ExportViewModel<T>
    {
     
        [Required]
        [StringLength(300, MinimumLength = 2, ErrorMessage = "Export reason cannot be less than 2 characters or exceed 300 characters")]
        public string ExportReason { get; set; }
        [Required]
        [DobValidation]
        public DateTime ExportDate { get; set; }
        [Required]
        public List<ItemViewModel<T>> Items { get; set; }     


    }
}
