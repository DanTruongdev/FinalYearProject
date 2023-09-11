using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OnlineShopping.Data;
using OnlineShopping.Hubs;
using OnlineShopping.Models.Funiture;
using OnlineShopping.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace OnlineShopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopOwnerController : ControllerBase
    {
        private readonly IHubContext<SignalHub> _hubContext;
        private readonly ApplicationDbContext _dbContext;
        public ShopOwnerController(IHubContext<SignalHub> hubContext, ApplicationDbContext dbContext)
        {
            _hubContext = hubContext;
            _dbContext = dbContext;
        }
        [HttpGet("customize-furniture-request")]
        public async Task<IActionResult> GetCustomizeFurnitureRequest(string status)
        {
            List<CustomizeFurniture> customizeFurnitures = new List<CustomizeFurniture>();
            if (status.Equals("All")) customizeFurnitures = await _dbContext.CustomizeFurnitures.ToListAsync();
            else customizeFurnitures = await _dbContext.CustomizeFurnitures.Where(cf => cf.Result.Status.Equals(status)).ToListAsync();
            if (customizeFurnitures.Count == 0) return NotFound($"There is no customize furniture with {status} status");       
            try
            {
                var response = customizeFurnitures.Select(cf => new
                {
                    CustomizeFurnitureId = cf.CustomizeFurnitureId,
                    CustomerId = cf.CustomerId,
                    CustomizeFurnitureName = cf.CustomizeFurnitureName,
                    ColorId = cf.ColorId,
                    Height = cf.Height,
                    Width = cf.Width,
                    Length = cf.Length,
                    WoodId = cf.WoodId,
                    Quantity = cf.Quantity,
                    DesiredCompletionDate = cf.DesiredCompletionDate,
                    CreationDate = cf.CreationDate,
                    Images = cf.Attachments.Where(a => a.Type.Equals("Images")).Select(a => new
                    {
                        AttachmentName = a.AttachmentName,
                        Path = a.Path
                    }),
                    Videos = cf.Attachments.Where(a => a.Type.Equals("Videos")).Select(a => new
                    {
                        AttachmentName = a.AttachmentName,
                        Path = a.Path
                    }),
                }).ToList();
                return Ok(response);
            } catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response() { Status = "Error", Message = "An error occurs during fetch data" });
            }
            
          
        }

    }
}
