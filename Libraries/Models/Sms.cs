

namespace OnlineShopping.Libraries.Models
{
    public class Sms
    {
        public List<string> To { get; set; }
        public string Content { get; set; }
        public Sms(IEnumerable<string>  to, string content) {
            To = new List<string>();
            To.AddRange(to);
            Content = content;      
        }
    }
}
