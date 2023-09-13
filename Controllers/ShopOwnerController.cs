using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OnlineShopping.Data;
using OnlineShopping.Hubs;
using OnlineShopping.Models.Customer;
using OnlineShopping.Models.Funiture;
using OnlineShopping.ViewModels;
using OnlineShopping.ViewModels.Furniture;
using System.ComponentModel.DataAnnotations;
using System.IO.Pipes;
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

                                                                             //Customer request

        [HttpGet("customer-requests/customize-furnitures")]
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
                    Status = cf.Result.Status
                }).ToList();
                return Ok(response);
            } catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response() { Status = "Error", Message = "An error occurs during fetch data" });
            }
        }

        [HttpPut("customize-requests/customize-furnitures/confirm")]
        public async Task<IActionResult> ConfirmCustomizeFurnitureRequest([FromForm] ResultViewModel userInput)
        {
            CustomizeFurniture customizeFurniture = await _dbContext.CustomizeFurnitures.FindAsync(userInput.CustomizeFurnitureId);
            if (customizeFurniture == null) return NotFound("The customize furniture not found");
            if (userInput.Status.Equals(customizeFurniture.Result.Status)) return  StatusCode(StatusCodes.Status406NotAcceptable,
                    new Response() { Status = "Error", Message = $"Custom furniture status has been {userInput.Status} already"});
            Result result = customizeFurniture.Result;
            result.Status = userInput.Status;
            result.Reason = userInput.Reason;
            result.ActualCompletionDate = userInput.ActualCompletionDate;
            result.CustomizeFurnitureId = userInput.CustomizeFurnitureId;
            result.ExpectedPrice = userInput.ExpectedPrice;
            string content = userInput.Status.Equals("Accepted") ?
                "Your customize furniture status has been accepted, Now you can use it to order" :
                "Your customize furniture status has been not accepted, Go to your customize furniture to see details";
            Announcement newAnnoucement = new Announcement() 
            { 
                UserId = customizeFurniture.CustomerId, 
                Title = "Your customize furniture has been changed status",
                Content = content,
                CreationDate = DateTime.Now
            };
            await _dbContext.AddAsync(newAnnoucement);
            _dbContext.UpdateRange(customizeFurniture, result);
            await _dbContext.SaveChangesAsync();
            return Ok("Confirm customize furniture successfully");
        }
        //[HttpGet("customize-requests/waranties")]
        //public async Task<IActionResult> GetCustomizeFurnitureRequest(string status)
        //{
        //    return Ok();
        //}
        //Supported method

    }
}
