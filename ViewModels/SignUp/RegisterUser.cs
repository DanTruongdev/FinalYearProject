using Bogus.DataSets;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels.SignUp
{
    public class RegisterUser
    {

        [Required(ErrorMessage = "Firtname is required")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "Lastname is required")]
        public string? LastName { get; set; }
        public DateTime? DoB { get; set; }
        public string? Gender { get; set; }
        public string? PhoneNumbers { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(40, MinimumLength = 6)]
        public string? Password { get; set; }
    }
}
