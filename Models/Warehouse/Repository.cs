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
        public virtual Address? Address { get; set; }
        public virtual ICollection<Import> Imports { get; set; }
        public virtual ICollection<FurnitureRepository>? FurnitureRepositories { get; set; }
        public virtual ICollection<MaterialRepository>? MaterialRepositories { get; set; }
    }
}
