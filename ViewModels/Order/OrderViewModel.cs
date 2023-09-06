using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels.Order
{
    public class OrderViewModel
    {
        [Required(ErrorMessage = "AddressId is required")]
        public int AddressId { get; set; }
        [Required(ErrorMessage = "Order items are required")]
        public List<CheckoutItem> Items { get; set; }
        public int? UsedPoint { get; set; }
        [Required(ErrorMessage = "PaymentId is required")]
        public int PaymentId { get; set; }     
        public string? Note { get; set; }
        
    }
}
