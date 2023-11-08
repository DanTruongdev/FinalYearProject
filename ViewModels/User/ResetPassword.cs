using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels.User
{
    public class ResetPassword
    {
        [Required]
        public string? Password { get; set; }
        [Compare("Password", ErrorMessage = "Not matched")]
        public string? ConfirmPassword { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
    }
}
