using System.ComponentModel.DataAnnotations;
namespace OnlineShopping.Models.Warehouse
{
    public class Supplier
    {
        [Key]
        public int SupplierId { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 2,
          ErrorMessage = "Suplier name cannot be less than 2 characters or exceed 30 characters")]
        public string SupplierName { get; set; }

        [Required]
        public int SupplierAddressId { get; set; }
        public string? SupplierImage { get; set; }
        [Required]
        public string SupplierEmail { get; set; }
        [Required]
        public string SupplierPhoneNums { get; set; }
      
        //
        public virtual Address? Address { get; set; }    
        public virtual ICollection<Material> Materials { get; set; }
    }
}
