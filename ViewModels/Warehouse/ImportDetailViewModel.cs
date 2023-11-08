using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.SignalR.Hubs;

namespace OnlineShopping.ViewModels.Warehouse
{
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

