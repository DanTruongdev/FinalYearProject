using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels.User
{
    public class Login2fa
    {
        [Required]
        public string token { get; set; }
        [Required]
        public string email { get; set; }
    }
}
