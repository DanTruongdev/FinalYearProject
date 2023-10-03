using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels.Warehouse
{
    public class ConfirmImportViewModel
    {
        [Required]
        [FileValidation]
        public IFormFile BillImage { get; set; }
        [Required]
        public DateTime DeliveryDate { get; set; }
    }
}
