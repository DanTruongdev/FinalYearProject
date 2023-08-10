using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels
{
    public class FurnitureSpecificationViewModel
    {       
        public int FurnitureSpecificationId { get; set; }   
        public int FurnitureId { get; set; }
        public string FurnitureName { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
        public string Color { get; set; }
        public string Wood { get; set; }
        public string Description { get; set; }
    }
}
