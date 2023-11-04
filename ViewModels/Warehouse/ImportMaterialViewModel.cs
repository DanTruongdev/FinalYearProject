using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels.Warehouse
{
    public class ImportMaterialViewModel
    {       
        [Required]
        public int RepositoryId { get; set; }
            [Required]
        public List<ImportDetailViewModel> Items { get; set; }

        public class ImportDetailViewModel
        {

            [Required]
            public int MaterialId { get; set; }
            [Required]
            [Range(1, int.MaxValue, ErrorMessage = "Quantity of material must be greater than 0")]
            public int Quantity { get; set; }
            [StringLength(200, MinimumLength = 2, ErrorMessage = "Note  cannot be less than 2 characters or exceed 200 characters")]
            public string? Note { get; set; }

        }
    }
}
