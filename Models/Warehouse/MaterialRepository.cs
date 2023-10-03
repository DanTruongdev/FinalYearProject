using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.Warehouse
{
    public class MaterialRepository
    {
        [Required]
        public int RepositoryId { get; set; }
        [Required]
        public int MaterialId { get; set; }
        [Required]
        public int Available { get; set; }
        //
        public virtual Repository Repository { get; set; }
        public virtual Material Material { get; set; }
    }
}
