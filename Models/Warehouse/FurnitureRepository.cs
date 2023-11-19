using OnlineShopping.Models.Funiture;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.Warehouse
{
    public class FurnitureRepository
    {
        [Required]
        public int RepositoryId { get; set; }
        [Required]
        public string FurnitureSpecificationId { get; set; }
        [Required]
        public int Available { get; set; }
        //
        public virtual Repository Repository { get; set; }
        public virtual FurnitureSpecification FurnitureSpecification { get; set; }
    }
}
