using OnlineShopping.Models.Funiture;

namespace OnlineShopping.Models.Purchase
{
    public class WishListDetail
    {
        public int WishListDetailId { get; set; }
        public int WishListId { get; set; }
        public int FurnitureId { get; set; }
        //
        public WishList WishList { get; set; }
        public Furniture Furniture { get; set; }
    }
}
