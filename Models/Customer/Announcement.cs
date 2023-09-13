using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.Customer
{
    public class Announcement
    {
        [Key]
        public int AnnouncementId { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreationDate { get; set; }
        //
        public virtual User User { get; set; }

    }
}
