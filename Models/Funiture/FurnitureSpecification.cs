using OnlineShopping.Models.Customer;
using OnlineShopping.Models.Gallary;
using OnlineShopping.Models.Purchase;
using OnlineShopping.Models.Warehouse;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.Funiture
{
    public class FurnitureSpecification
    {
        [Key]
        public int FurnitureSpecificationId { get; set; }
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
        public Furniture Furniture { get; set; }
        public Color? Color { get; set; }
        public Wood? Wood { get; set; }
        public ICollection<Attachment> Attachment { get; set; }
        public ICollection<FurnitureRepository> FurnitureRepositories { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
        public ICollection<CartDetail> CartDetails { get; set; }
        public ICollection<FurnitureOrderDetail> FurnitureOrderDetails { get; set; }
    }
}
