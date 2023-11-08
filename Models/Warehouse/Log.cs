using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.Warehouse
{
    public class Log
    {
         [Key]
         public int LogId {get; set;}
         [Required]
         public string UserId { get; set; }
         [Required]
         public string Activity  {get; set;}
         [Required]
         public DateTime Date { get; set; }

        //
        public virtual User User { get; set; }

    }
}
