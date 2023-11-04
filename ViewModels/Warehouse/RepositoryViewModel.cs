using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels.Warehouse
{
    public class RepositoryViewModel
    {  
        [Required]
        [StringLength(30, MinimumLength = 2,
          ErrorMessage = "Repository name cannot be less than 2 characters or exceed 50 characters")]
        public string RepositoryName { get; set; }
        
        [Required]
        [StringLength(60, MinimumLength = 2,
          ErrorMessage = "Ward cannot be less than 2 characters or exceed 60 characters")]
        public string Street { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 2,
          ErrorMessage = "Ward cannot be less than 2 characters or exceed 30 characters")]
        public string Ward { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 2,
          ErrorMessage = "District cannot be less than 2 characters or exceed 20 characters")]
        public string District { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 2,
          ErrorMessage = "Province cannot be less than 2 characters or exceed 20 characters")]
        public string Province { get; set; }

        [Required]
        [Range(10, double.MaxValue, ErrorMessage = "Capacity must be greater than 10")]
        public double Capacity { get; set; }
        [Required]
        public bool IsFull { get; set; }
      
    }
}
