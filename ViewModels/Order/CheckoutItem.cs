using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels.Order
{
    public class CheckoutItem
    {
        public string ItemId { get; set; }
        [Range(0, 100 , ErrorMessage = "Quantity must be greater than 0 and less than 100")]
        public int Quantity { get; set; }       
    }
}
