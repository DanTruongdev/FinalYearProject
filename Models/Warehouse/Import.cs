using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShopping.Models.Warehouse
{
    public class Import
    {
        [Key]
        public int ImportId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int RepositoryId { get; set; }
        public string? BillImage { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        public string Status { get; set; } //2 status: Processing and Delivered.
        //
        public virtual ICollection<ImportDetail> ImportDetails { get; set; }
        public virtual Repository Repository { get; set; }
        public virtual User User { get; set; }
    }
}
