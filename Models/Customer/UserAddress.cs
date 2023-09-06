using OnlineShopping.Models.Purchase;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.Customer
{
    public class UserAddress
    {
        public string UserId { get; set; }
        public int AddressId { get; set; }
        public string AddressType { get; set; }
        //
        public virtual User User { get; set; }
        public virtual Address Address { get; set; }
    }
}
