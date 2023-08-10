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
        public User User { get; set; }
        public Address Address { get; set; }
    }
}
