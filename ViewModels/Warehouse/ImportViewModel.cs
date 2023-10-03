using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace OnlineShopping.ViewModels.Warehouse
{
    public class ImportViewModel
    {
        [Required]
        public int RepositoryId { get; set; }
        [Required]
        public IList<ImportDetailViewModel> Items { get; set; }

    }
}
