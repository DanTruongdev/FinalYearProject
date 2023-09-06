using System.ComponentModel.DataAnnotations;
namespace OnlineShopping.Models.Warehouse
{
    public class Suplier
    {
        [Key]
        public int SuplierId { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 2,
          ErrorMessage = "Suplier name cannot be less than 2 characters or exceed 30 characters")]
        public string SuplierName { get; set; }

        [StringLength(60, MinimumLength = 2,
          ErrorMessage = "Suplier address cannot be less than 2 characters or exceed 60 characters")]
        public int? SuplierAddressId { get; set; }
        public string? SuplierImage { get; set; }
        [Required]
        public string SuplierEmail { get; set; }
        [Required]
        public string SuplierPhoneNums { get; set; }
        //
        public virtual Address? Address { get; set; }    
        public virtual ICollection<Material> Materials { get; set; }
    }
}
