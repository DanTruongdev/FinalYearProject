using OnlineShopping.Models.Funiture;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.Warehouse
{
    public class FurnitureRepository
    {
        [Required]
        public int RepositoryId { get; set; }
        [Required]
        public int FurnitureSpecificationId { get; set; }
        [Required]
        public int Available { get; set; }
        //
        public Repository Repository { get; set; }
        public FurnitureSpecification FurnitureSpecification { get; set; }
    }
}
