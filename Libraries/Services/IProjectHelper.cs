using OnlineShopping.Models;
using OnlineShopping.Models.Purchase;
using OnlineShopping.ViewModels.VNPAY;
using System.IdentityModel.Tokens.Jwt;

namespace OnlineShopping.Libraries.Services
{
    public interface IProjectHelper
    {

        public bool Refund(Order order);
        public VnPayTransition GetTransition(Order order);
        public string UrlPayment(int typePayment, int orderId);
        public Task<bool> VerifyPhoneNum(string phoneNums, string totpCode);
        public JwtSecurityToken GenerateJWTToken(User user, IList<string> roles);
        public Task<bool> CreateUserInfor(string email);
        public string CheckInforVerify(User customer);
        public Task<bool> CreateLogAsync(string assistantId, string activity);

        public Task<bool> CreateAnnouncementAsync(string assistantId, string title, string content);
        public bool CheckUserAddress(User user);
        public string FilterBadWords(string input);

    }
}
