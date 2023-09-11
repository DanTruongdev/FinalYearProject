using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace OnlineShopping.Models.Funiture
{
    public class Result
    {
        [Key]
        public int ResultId { get; set; }
        [Required]
        public int CustomizeFurnitureId { get; set; }
        [AllowNull]  
        public DateTime? ActualCompletionDate { get; set; }
        [AllowNull]
        public double? ExpectedPrice { get; set; }
        [Required]
        public string Status { get; set; }
        [AllowNull]
        public string? Reason { get; set; }
          //
          public virtual CustomizeFurniture CustomizeFurniture { get; set; }
    }
}
