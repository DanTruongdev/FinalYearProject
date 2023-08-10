using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OnlineShopping.Data;
using OnlineShopping.Hubs;

namespace OnlineShopping.Controllers
{
    [ApiController]
    public class ShopOwnerController : ControllerBase
    {
        private readonly IHubContext<SignalHub> _hubContext;
        private readonly ApplicationDbContext _context;
        public ShopOwnerController(IHubContext<SignalHub> hubContext, ApplicationDbContext context)
        {
            _hubContext = hubContext;
            _context = context;
        }


    }
}
