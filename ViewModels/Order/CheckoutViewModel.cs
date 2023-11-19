using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels.Order
{
    public class CheckoutViewModel
    {     
        public int FurnitureId { get; set; }
        public string FurnitureName { get; set; }
        public string FurnitureSpecificationId { get; set; }
        public string FurnitureSpecificationName { get; set; }
        public int Quantity { get; set; }
        public double Cost { get; set; }
    }
}
