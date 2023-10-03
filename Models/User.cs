using Microsoft.AspNetCore.Identity;
using OnlineShopping.Models.Customer;
using OnlineShopping.Models.Funiture;
using OnlineShopping.Models.Purchase;
using OnlineShopping.Models.Warehouse;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShopping.Models
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(30, MinimumLength = 2,
          ErrorMessage = "First name cannot be less than 2 characters or exceed 30 characters")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 2,
          ErrorMessage = "Last name cannot be less than 2 characters or exceed 30 characters")]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DoB { get; set; }

        public string Gender { get; set; }
        public string? Avatar { get; set; }
        public double? Spent { get; set; } = 0;
        public double? Debit { get; set; } = 0;
        public DateTime CreationDate { get; set; }
        public DateTime? LatestUpdate { get; set; }
        public bool IsActivated { get; set; }


        // All
        public virtual ICollection<Announcement>? Announcements { get; set; }
        //Customer
        public virtual Point? Point { get; set; }
        public virtual ICollection<Feedback>? Feedbacks { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
        public virtual WishList WishList { get; set; }
        public virtual Cart Cart { get; set; }
        public virtual ICollection<UserAddress> UserAddresses { get; set; }
        public virtual ICollection<CustomizeFurniture>? CustomizeFurnitures { get; set; }
        public virtual ICollection<Warranty>? Warranties { get; set; }
        //Assistant
        public virtual ICollection<Import>? Imports { get; set; }
    }
}
