using OnlineShopping.Libraries.Models;

namespace OnlineShopping.Libraries.Services
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}
