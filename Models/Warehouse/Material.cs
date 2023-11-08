using System.ComponentModel.DataAnnotations;


namespace OnlineShopping.Models.Warehouse
{
    public class Material
    {
        [Key]
        public int MaterialId { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 2,
           ErrorMessage = "Material name cannot be less than 2 characters or exceed 50 characters")]
        public string MaterialName { get; set; }
        [Required]
        public int MaterialPrice { get; set; }    
        public string? MaterialImage { get; set; }
        public string Description { get; set; }
        public int? DefaultSuplierId { get; set; }
        //     
        public virtual Supplier? DefaultSuplier { get; set; }
        public virtual ICollection<ImportDetail> ImportDetails { get; set; }
        public virtual ICollection<MaterialRepositoryHistory> MaterialRepositoryHistories { get; set; }
        public virtual ICollection<MaterialRepository> MaterialRepositories { get; set; }
    }
}
