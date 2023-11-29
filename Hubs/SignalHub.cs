using Microsoft.AspNetCore.SignalR;
using OnlineShopping.Hubs.Models;
using OnlineShopping.ViewModels.User;

namespace OnlineShopping.Hubs
{
    public class SignalHub:Hub
    {
        public async Task SendJWTToken(JwtToken jwtToken)
        {
            await Clients.All.SendAsync("ReceiveJWTToken", jwtToken);
        }
        public async Task SendResetPassword(ResetPassword resetPassword)
        {
            await Clients.All.SendAsync("ReceiveResetPassword", resetPassword);
        }
    }
}
