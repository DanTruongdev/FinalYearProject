using OnlineShopping.Models.Funiture;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShopping.Models.Warehouse
{
    public class FurnitureRepositoryHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FurnitureRepositoryHistoryId { get; set; }
        [Required]
        public int RepositoryId { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string AssistantId { get; set; }
        [Required]
        public string FurnitureSpecificationId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        [StringLength(300, MinimumLength = 2, ErrorMessage = "Description cannot be less than 2 characters or exceed 300 characters")]
        public string Description { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }

        //
        public virtual User Assistant { get; set; }
        public virtual Repository Repository { get; set; }
        public virtual FurnitureSpecification FurnitureSpecification { get; set; }

        
    }
}
