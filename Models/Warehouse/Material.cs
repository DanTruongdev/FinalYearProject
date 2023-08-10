using OnlineShopping.Models.Gallary;
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
        public string Description { get; set; }
        public int? DefaultSuplierId { get; set; }
        //
        public ICollection<Attachment> Attachments { get; set; }
        public Suplier? DefaultSuplier { get; set; }
        public ICollection<ImportDetail> ImportDetails { get; set; }
        public ICollection<MaterialRepository> MaterialRepositories { get; set; }
    }
}
