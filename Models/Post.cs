using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 2,
          ErrorMessage = "Post title cannot be less than 2 characters or exceed 100 characters")]
        public string Title { get; set; }
        [Required]
        [MinLength(2, ErrorMessage = "Post content cannot be less than 2 characters")]
        public string Content { get; set; }
        [Required]
        public string Type { get; set; } //Tip or New
        public string? Image { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }    
        public DateTime? LatestUpdate { get; set;}
        [Required]
        public string AuthorId { get; set; }
        //
        public virtual User Author { get; set; }
    }
}
