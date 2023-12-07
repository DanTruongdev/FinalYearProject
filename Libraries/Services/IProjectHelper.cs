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
        public string CheckUserInfor(User user);
        public Task<bool> CreateLogAsync(string assistantId, string activity);

        public Task<bool> CreateAnnouncementAsync(User user, string title, string content);
        public string FilterBadWords(string input);
        public Task<bool> UpdateCustomerSpentAsync(Order order);

        public Task<bool> CreatePointHistoryAsync(User user,int point, string description);
    }
}
