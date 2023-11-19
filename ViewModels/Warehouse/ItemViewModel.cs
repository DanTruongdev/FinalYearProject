using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels.Warehouse
{
    public class ItemViewModel<T>
    {
        [Required]
        public T Id { get; set; }
        [Required]
        [Range(1, 1000, ErrorMessage = "Quantity must between 1 and 1000")]
        public int Quantity { get; set; }
    }
}
