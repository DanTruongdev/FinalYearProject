using OnlineShopping.Models.Funiture;
using OnlineShopping.Models.Warehouse;

namespace OnlineShopping.ViewModels.Order
{
    public class FurnitureOrderItem
    {
        public FurnitureSpecification FurnitureSpecification { get; set; }
        public FurnitureRepository FurnitureRepository { get; set;}
        public int Quantity { get; set; }
    }
}
