using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels.User
{
    public class EditInforViewModel
    {
        [StringLength(30, MinimumLength = 2,
        ErrorMessage = "LastName cannot be less than 2 characters or exceed 50 characters")]
        public string? FirstName { get; set; }
        [StringLength(30, MinimumLength = 2,
        ErrorMessage = "LastName cannot be less than 2 characters or exceed 50 characters")]
        public string? LastName { get; set; }
        [DobValidation(ErrorMessage = " The Dob must be less than current date time")]
        public DateTime? DoB { get; set; }

        [GenderValidation]
        public string? Gender { get; set; }

        [FileValidation]
        public IFormFile? Image { get; set; }
    }

    public class EditAllInforViewModel
    {
        [Required]
        public string UserId { get; set; }

        [StringLength(30, MinimumLength = 2,
        ErrorMessage = "LastName cannot be less than 2 characters or exceed 50 characters")]
        public string? FirstName { get; set; }

        [StringLength(30, MinimumLength = 2,
        ErrorMessage = "LastName cannot be less than 2 characters or exceed 50 characters")]
        public string? LastName { get; set; }
        [DobValidation(ErrorMessage = " The Dob must be less than current date time")]
        public DateTime? DoB { get; set; }

        [GenderValidation]
        public string? Gender { get; set; }

        [FileValidation]
        public IFormFile? Image { get; set; }
    }
}
