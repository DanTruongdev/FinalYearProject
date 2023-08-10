using OnlineShopping.Libraries.Models;

namespace OnlineShopping.Libraries.Services
{
    public interface ISMSService
    {
        public string GetUserInfor();
        public string SendSMS(Sms sms);
    }
}
