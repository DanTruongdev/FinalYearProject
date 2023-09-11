using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShopping.Models.Warehouse
{
    public class ImportDetail
    {
        [Key]
        public int ImportDetailId { get; set; }
        [Required]
        public int ImportId { get; set; }
        [Required]
        public int MaterialId { get; set; }
        [NotMapped]
        public string Suplier { get; set; }
        [Required]
        public int Quantity { get; set; }
        [NotMapped]
        public double Cost { get; set; }
        public string Note { get; set; }
        //
        public virtual Import Import { get; set; }
        public virtual Material Material { get; set; }
    }
}
