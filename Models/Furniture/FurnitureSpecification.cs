using OnlineShopping.Models.Customer;
using OnlineShopping.Models.Gallery;
using OnlineShopping.Models.Purchase;
using OnlineShopping.Models.Warehouse;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.Funiture
{
    public class FurnitureSpecification
    {
        [Key]
        public string FurnitureSpecificationId { get; set; }
        [Required]
        public string FurnitureSpecificationName { get; set; }
        [Required]
        public int FurnitureId { get; set; }
        [Required]
        public double Height { get; set; }
        [Required]
        public double Width { get; set; }
        [Required]
        public double Length { get; set; }      
        public int? ColorId { get; set; }
        public int? WoodId { get; set; }
        [Required]
        public double Price { get; set; }
        public string Description { get; set; }
        ////
        public virtual Furniture Furniture { get; set; }
        public virtual Color? Color { get; set; }
        public virtual Wood? Wood { get; set; }
        public virtual ICollection<FurnitureSpecificationAttachment> Attachments { get; set; }
        public virtual ICollection<FurnitureRepository> FurnitureRepositories { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<CartDetail> CartDetails { get; set; }
        public virtual ICollection<FurnitureOrderDetail> FurnitureOrderDetails { get; set; }
        public virtual ICollection<FurnitureRepositoryHistory> FurnitureRepositoryHistories { get; set; }
    }
}
