using OnlineShopping.Models.Funiture;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.Warehouse
{
    public class Repository
    {
        [Key]
        public int RepositoryId { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 2,
          ErrorMessage = "Repository name cannot be less than 2 characters or exceed 50 characters")]
        public string RepositoryName { get; set; }
        public int? AddressId { get; set; }
        public double Capacity { get; set; }
        public DateTime CreationDate { get; set; }
        //
        public Address? Address { get; set; }
        public ICollection<Import> Imports { get; set; }
        public ICollection<FurnitureRepository>? FurnitureRepositories { get; set; }
        public ICollection<MaterialRepository>? MaterialRepositories { get; set; }
    }
}
