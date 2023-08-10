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
        public string Status { get; set; }


        // All
        public ICollection<Announcement>? Announcements { get; set; }
        //Customer
        public Point? Point { get; set; }
        public ICollection<Feedback>? Feedbacks { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public WishList WishList { get; set; }
        public Cart Cart { get; set; }
        public ICollection<UserAddress> UserAddresses { get; set; }
        public ICollection<CustomizeFurniture>? CustomizeFurniture { get; set; }
        public ICollection<WarrantySchedule>? WarrantySchedules { get; set; }
        //Assistant
        public ICollection<Import>? Imports { get; set; }
    }
}
