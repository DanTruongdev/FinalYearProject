using Microsoft.AspNetCore.SignalR;
using OnlineShopping.Data;
using OnlineShopping.Hubs;

namespace OnlineShopping.Controllers
{
    public class SupplierController
    {
        private readonly IHubContext<SignalHub> _hubContext;
        private readonly ApplicationDbContext _dbContext;
        public SupplierController(IHubContext<SignalHub> hubContext, ApplicationDbContext dbContext)
        {
            _hubContext = hubContext;
            _dbContext = dbContext;
        }
        
    }
}
