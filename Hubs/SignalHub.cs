using Microsoft.AspNetCore.SignalR;
using OnlineShopping.Hubs.Models;

namespace OnlineShopping.Hubs
{
    public class SignalHub:Hub
    {
        public async Task SendJWTToken(JwtToken jwtToken)
        {
            await Clients.All.SendAsync("ReceiveJWTToken", jwtToken);
        }
    }
}
