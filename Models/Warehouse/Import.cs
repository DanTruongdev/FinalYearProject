﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShopping.Models.Warehouse
{
    public class Import
    {
        [Key]
        public int ImportId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int RepositoryId { get; set; }
        [NotMapped]
        public string Address { get; set; }
        [NotMapped]
        public double TotalCost { get; set; }
        public string Status { get; set; }
        //
        public virtual ICollection<ImportDetail> ImportDetails { get; set; }
        public virtual Repository Repository { get; set; }
        public virtual User User { get; set; }
    }
}
