using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Libraries.Models;
using OnlineShopping.Libraries.Services;
using OnlineShopping.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using OnlineShopping.Models.Funiture;
using OnlineShopping.Models;
using OnlineShopping.Models.Purchase;
using OnlineShopping.Models.Customer;
using Newtonsoft.Json;
using OnlineShopping.ViewModels.Order;
using System.Web;
using OnlineShopping.ViewModels.Feedback;
using OnlineShopping.Models.Gallery;
using Castle.Core.Internal;
using OnlineShopping.ViewModels.Furniture;
using OnlineShopping.ViewModels.Warranty;
using OnlineShopping.Data;
using Castle.Core;
using System.Web.Http.Description;
using System.Net.Mail;

namespace OnlineShopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        private readonly IProjectHelper _projectHelper;
        private readonly ICloudinaryService _cloudinaryService;
        public CustomerController(ApplicationDbContext context, UserManager<User> userManager, IConfiguration config, IProjectHelper projectHelper, ICloudinaryService cloudinaryService)
        {
            _dbContext = context;
            _userManager = userManager;
            _config = config;
            _projectHelper = projectHelper;
            _cloudinaryService = cloudinaryService;
        }



        //FURNITURE
        [HttpGet("furnitures")]
        public async Task<IActionResult> GetAllFurniture()
        {
            bool isLoggedIn = false;
            ICollection<WishListDetail> userWishlist = new List<WishListDetail>();
            var email = User.FindFirstValue(ClaimTypes.Email);
            var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            if (loggedInUser != null) isLoggedIn = true;
            if (isLoggedIn)
            {
                userWishlist = loggedInUser.WishList == null ? new List<WishListDetail>() : loggedInUser.WishList.WishListDetails.ToList();
            }

            var furnitures = await _dbContext.Furnitures.Include(f => f.FurnitureSpecifications).Include(f => f.Collection).Include(f => f.Category).Include(f => f.Label).AsNoTracking().Select(f => new
            {
                FurnitureId = f.FurnitureId,
                FurnitureName = f.FurnitureName,
                VoteStar = f.VoteStar,
                Sold = f.Sold,
                Label = f.Label == null ? string.Empty : f.Label.LabelName,
                CategoryId = f.CategoryId,
                Category = f.Category.CategoryName,
                AppropriateRoom = f.AppopriateRoom,
                Price = f.FurnitureSpecifications.Any() ? f.FurnitureSpecifications.Min(fs => fs.Price) : 0,
                Image = f.FurnitureSpecifications.Any(fs => fs.Attachments.Any()) ? _cloudinaryService.GetSharedUrl(f.FurnitureSpecifications.FirstOrDefault().Attachments.FirstOrDefault().Path) : "",
                CollectionId = f.CollectionId.HasValue ? f.CollectionId.Value.ToString() : "None",
                Collection = f.Collection.CollectionName,
                isLike = isLoggedIn && userWishlist.FirstOrDefault(w => w.FurnitureId == f.FurnitureId) != null ? true : false
            }).ToListAsync();

            //var response = furnitures.Select( async f => new
            //{
            //    FurnitureId = f.FurnitureId,
            //    FurnitureName = f.FurnitureName,
            //    VoteStar = f.VoteStar,
            //    Sold = f.Sold,
            //    Label = f.Label == null ? string.Empty : f.Label.LabelName,
            //    CategoryId = f.CategoryId,
            //    Category = f.Category.CategoryName,
            //    AppropriateRoom = f.AppopriateRoom,
            //    Price = f.FurnitureSpecifications.Count > 0 ? f.FurnitureSpecifications.Min(fs => fs.Price) : 0,
            //    //Image = f.FurnitureSpecifications.Count > 0 && f.FurnitureSpecifications.FirstOrDefault().Attachments.Count > 0 ?
            //    //         _dropboxService.GetDownloadLinkAsync(f.FurnitureSpecifications.FirstOrDefault().Attachments.FirstOrDefault().Path).Result
            //    //        : String.Empty,
            //    Image = f.FurnitureSpecifications.IsNullOrEmpty() ? "" : await GetImageById(f.FurnitureSpecifications.First().FurnitureSpecificationId, attachments),
            //    CollectionId = f.CollectionId.HasValue ? f.CollectionId.Value.ToString() : "None",
            //    Collection = f.Collection.CollectionName,
            //    //isLike = isLoggedIn && userWishlist.FirstOrDefault(w => w.FurnitureId == f.FurnitureId) != null ? true : false
            //});

            return Ok(furnitures);
        }

        [HttpGet("furnitures/{id}")]

        public async Task<IActionResult> GetFurnitureSpecificationById(int id)
        {
            var furnitureSpecifications = await _dbContext.FurnitureSpecifications.Where(fs => fs.FurnitureId == id).ToListAsync();
            if (furnitureSpecifications.Count == 0)
            {
                return NotFound();
            }
            var responses = new List<object>();
            foreach (var fs in furnitureSpecifications)
            {

                var response = new
                {

                    FurnitureId = fs.FurnitureId,
                    FurnitureName = fs.Furniture.FurnitureName,
                    FurnitureSpecificationId = fs.FurnitureSpecificationId,
                    FurnitureSpecificationName = fs.FurnitureSpecificationName,
                    Height = fs.Height,
                    Width = fs.Width,
                    Length = fs.Length,
                    Color = fs.Color.ColorName,
                    Wood = fs.Wood.WoodType,
                    Price = fs.Price,
                    Description = fs.Description,
                    Available = _dbContext.FurnitureRepositories.FirstOrDefault(fr => fr.RepositoryId == 1 && fr.FurnitureSpecificationId == fs.FurnitureSpecificationId) == null ? 0 :
                                _dbContext.FurnitureRepositories.FirstOrDefault(fr => fr.RepositoryId == 1 && fr.FurnitureSpecificationId == fs.FurnitureSpecificationId).Available,
                    Images = fs.Attachments.Where(a => a.Type.Equals("images")).Select( a => new
                    {
                        AttachmentName = a.AttachmentName,
                        Path = _cloudinaryService.GetSharedUrl(a.Path)
                    }),
                    Videos = fs.Attachments.Where(a => a.Type.Equals("videos")).Select( a => new
                    {
                        AttachmentName = a.AttachmentName,
                        Path = _cloudinaryService.GetSharedUrl(a.Path)
                    }),
                    Feedbacks = fs.Feedbacks.Select(fb => new
                    {
                        Customer = fb.Anonymous ? fb.Customer.FirstName.Substring(0, 2) + "****" : fb.Customer.FirstName + " " + fb.Customer.LastName,
                        Content = fb.Content,
                        VoteStar = fb.VoteStar,
                        CreationDate = fb.CreationDate
                    })
                };
                responses.Add(response);
            }
            return Ok(responses);
        }

        [HttpGet("furnitures/search")]

        public async Task<IActionResult> SearchFurnitureF(string keyword)
        {
            if (!keyword.IsNullOrEmpty()) keyword = keyword.ToUpper().Trim();
            var result = await _dbContext.Furnitures.Where(f => f.FurnitureName.ToUpper().Contains(keyword)).ToListAsync();
            if (result.IsNullOrEmpty()) return Ok(new List<Furniture>());
            var response = result.Select(f => new
            {
                FurnitureId = f.FurnitureId,
                FurnitureName = f.FurnitureName,
                VoteStar = f.VoteStar,
                Sold = f.Sold,
                Label = f.Label == null ? string.Empty : f.Label.LabelName,
                CategoryId = f.CategoryId,
                Category = f.Category.CategoryName,
                Available = f.FurnitureSpecifications.Count > 0 ? f.FurnitureSpecifications.Sum(fs => fs.FurnitureRepositories.Sum(fr => fr.Available)) : 0,
                AppropriateRoom = f.AppopriateRoom,
                Price = f.FurnitureSpecifications.Count > 0 ? f.FurnitureSpecifications.Min(fs => fs.Price) : 0,
                Image = f.FurnitureSpecifications.Count > 0 && f.FurnitureSpecifications.FirstOrDefault().Attachments.Count > 0 ?
                          _cloudinaryService.GetSharedUrl(f.FurnitureSpecifications.FirstOrDefault().Attachments.FirstOrDefault().Path)
                        : String.Empty,
                CollectionId = f.CollectionId.HasValue ? f.CollectionId.Value.ToString() : "None",
                Collection = f.Collection.CollectionName
            });
            return Ok(response);
        }

        [HttpGet("furnitures/filter")]

        public async Task<IActionResult> FurnitureFilter([FromQuery] FurnitureFilterViewModel filter)
        {
            filter.MaxCost = filter.MaxCost == 0 ? double.MaxValue : filter.MaxCost;
            var funituresSpecifications = await _dbContext.FurnitureSpecifications.Where(fs => fs.Price >= filter.MinCost && fs.Price <= filter.MaxCost).ToListAsync();
            if (!filter.Category.IsNullOrEmpty()) funituresSpecifications = funituresSpecifications.Where(fs => fs.Furniture.Category.Equals(filter.Category)).ToList();
            if (filter.Star != 0 || filter.Star == null) funituresSpecifications = funituresSpecifications.Where(fs => fs.Furniture.VoteStar > filter.Star && fs.Furniture.VoteStar < filter.Star + 1).ToList();
            if (!filter.AppropriateRoom.IsNullOrEmpty()) funituresSpecifications = funituresSpecifications.Where(fs => fs.Furniture.AppopriateRoom.Equals(filter.AppropriateRoom)).ToList();
            if (!filter.Collection.IsNullOrEmpty()) funituresSpecifications = funituresSpecifications.Where(fs => fs.Furniture.Collection.Equals(filter.Collection)).ToList();
            var furnitures = funituresSpecifications.GroupBy(fs => fs.Furniture).Select(gr => gr.Key).ToList();
            var response = furnitures.Select( f => new
            {
                FurnitureId = f.FurnitureId,
                FurnitureName = f.FurnitureName,
                VoteStar = f.VoteStar,
                Sold = f.Sold,
                Label = f.Label == null ? string.Empty : f.Label.LabelName,
                CategoryId = f.CategoryId,
                Category = f.Category.CategoryName,
                Available = f.FurnitureSpecifications.Count > 0 ? f.FurnitureSpecifications.Sum(fs => fs.FurnitureRepositories.Sum(fr => fr.Available)) : 0,
                AppropriateRoom = f.AppopriateRoom,
                Price = f.FurnitureSpecifications.Count > 0 ? f.FurnitureSpecifications.Min(fs => fs.Price) : 0,
                Image = f.FurnitureSpecifications.Count > 0 && f.FurnitureSpecifications.FirstOrDefault().Attachments.Count > 0 ?
                          _cloudinaryService.GetSharedUrl(f.FurnitureSpecifications.FirstOrDefault().Attachments.FirstOrDefault().Path)
                        : String.Empty,
                CollectionId = f.CollectionId.HasValue ? f.CollectionId.Value.ToString() : "None",
                Collection = f.Collection.CollectionName
            });
            return Ok(response);
        }

        //CART
        [HttpGet("cart")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetCart()
        {

            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found ");
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            var cartDetail = await _dbContext.CartDetails.Include(cd => cd.FurnitureSpecifition).Include(cd => cd.FurnitureSpecifition.Furniture)
                .Where(cd => cd.CartId == loggedInUser.Cart.CartId).ToListAsync();
            var usercCart = cartDetail.Select(cd => new
            {
                CartDetailId = cd.CartDetailId,
                FurnitureId = cd.FurnitureSpecifition.Furniture.FurnitureId,
                FurnitureName = cd.FurnitureSpecifition.Furniture.FurnitureName,
                FurnitureSpecificationId = cd.FurnitureSpecificationId,
                FurnitureSpecificationName = cd.FurnitureSpecifition.FurnitureSpecificationName,
                UnitPrice = cd.FurnitureSpecifition.Price,
                Quantity = cd.Quantity,
                Cost = cd.Quantity * cd.FurnitureSpecifition.Price
            });
            return Ok(usercCart);
        }

        [HttpPost("cart/add")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> AddToCart([Required] string furnitureSpecificationId, [Required] int quantity)
        {
            if (furnitureSpecificationId == null || quantity == null) return BadRequest();
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found ");
            var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            var furnitureSpecification = await _dbContext.FurnitureSpecifications.FindAsync(furnitureSpecificationId);
            if (furnitureSpecification == null) return NotFound("The furniture not found");
            try
            {
                //kiem tra san pham da co trong gio hang chua
                var cartDetailExist = loggedInUser.Cart.CartDetails.FirstOrDefault(cd => cd.FurnitureSpecificationId.Equals(furnitureSpecificationId));
                if (cartDetailExist != null)
                {
                    if (quantity != 0)
                    {
                        cartDetailExist.Quantity = quantity;
                        _dbContext.Update(cartDetailExist);

                    }
                    else
                    {
                        _dbContext.Remove(cartDetailExist);
                        await _dbContext.SaveChangesAsync();
                        return Ok(new Response("Success", "Remove furniture from cart successfully"));
                    }
                }
                else
                {
                    if (quantity == 0)
                    {
                        return BadRequest(new Response("Error", "Cannot add the furniture with zero quantity to cart"));
                    }
                    var newCartDetail = new CartDetail()
                    {
                        CartId = loggedInUser.Cart.CartId,
                        FurnitureSpecificationId = furnitureSpecificationId,
                        Quantity = quantity,
                    };
                    await _dbContext.AddAsync(newCartDetail);
                    loggedInUser.Cart.CartDetails.Add(newCartDetail);
                }
                await _dbContext.SaveChangesAsync();
                return Ok(new Response("Success", "Add the furniture to cart successfully"));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response("Error", "An error occurs when adding furniture to cart"));
            }

        }

        [HttpDelete("cart/remove/{id}")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> DeleteCartItem(string id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found ");
            var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            var cartItems = loggedInUser.Cart.CartDetails.ToList();
            var deleteItem = cartItems.FirstOrDefault(c => c.FurnitureSpecificationId.Equals(id));
            if (deleteItem == null) return NotFound("This furniture is not in the cart");
            try
            {
                _dbContext.Remove(deleteItem);
                await _dbContext.SaveChangesAsync();
                return Ok("Remove furniture from cart successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", "Remove furniture from cart failed"));
            }
        }




        //ANNOUNCEMENT
        [HttpGet("announcements")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetAnnouncement()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found ");
            var user = await _userManager.FindByEmailAsync(email);
            var announcement = await _dbContext.Announcements.Where(a => a.UserId == user.Id).OrderByDescending(a => a.CreationDate).Select(a => new
            {
                Title = a.Title,
                Content = a.Content,
                Date = a.CreationDate

            }).ToListAsync();
            return Ok(announcement);
        }

        //WISHLIST

        [HttpGet("wish-list")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetWishlist()
        {


            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return Unauthorized(new Response("Error", "Logged in user not found "));
            var loggedInUser = await _userManager.FindByEmailAsync(email);

            if (loggedInUser.WishList == null)
            {
                WishList newWishList = new WishList
                {
                    CustomerId = loggedInUser.Id
                };
                await _dbContext.AddAsync(newWishList);
                await _dbContext.SaveChangesAsync();
                return Ok(new List<WishListDetail>());
            }

            var wishList = loggedInUser.WishList.WishListDetails;
            if (wishList.IsNullOrEmpty()) return Ok(new List<WishListDetail>());
            var furnitures = wishList.Select(w => w.Furniture);
            var response = furnitures.Select( f => new
            {
                FurnitureId = f.FurnitureId,
                FurnitureName = f.FurnitureName,
                VoteStar = f.VoteStar,
                Sold = f.Sold,
                Label = f.Label == null ? string.Empty : f.Label.LabelName,
                CategoryId = f.CategoryId,
                Category = f.Category.CategoryName,
                Available = f.FurnitureSpecifications.Count > 0 ? f.FurnitureSpecifications.Sum(fs => fs.FurnitureRepositories.Sum(fr => fr.Available)) : 0,
                AppropriateRoom = f.AppopriateRoom,
                Price = f.FurnitureSpecifications.Count > 0 ? f.FurnitureSpecifications.Min(fs => fs.Price) : 0,
                Image = f.FurnitureSpecifications.Count > 0 && f.FurnitureSpecifications.FirstOrDefault().Attachments.Count > 0 ?
                         _cloudinaryService.GetSharedUrl(f.FurnitureSpecifications.FirstOrDefault().Attachments.FirstOrDefault().Path)
                        : String.Empty,
                CollectionId = f.CollectionId.HasValue ? f.CollectionId.Value.ToString() : "None",
                Collection = f.Collection.CollectionName
            });
            return Ok(response);
        }


        [HttpPut("wish-list/toggle")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> ToggleWishlist(int furnitureId)

        {
            if (furnitureId == null) return BadRequest();
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found ");
            var loggedInUser = await _dbContext.Users.Include(u => u.WishList).FirstOrDefaultAsync(u => u.Email.Equals(email));
            var furniture = await _dbContext.Furnitures.FindAsync(furnitureId);
            if (furniture == null) return NotFound("The furniture not found");
            var wishListDetailExist = await _dbContext.WishListDetails.FirstOrDefaultAsync(w => w.WishListId == loggedInUser.WishList.WishlistId
                                         && w.FurnitureId == furnitureId);
            if (wishListDetailExist != null)
            {
                _dbContext.Remove(wishListDetailExist);
                loggedInUser.WishList.WishListDetails.Remove(wishListDetailExist);
                _dbContext.Update(loggedInUser);
                await _dbContext.SaveChangesAsync();
                return Ok("The furniture has been removed from wishlist");
            }
            else
            {
                var newWishlistItem = new WishListDetail()
                {
                    WishListId = loggedInUser.WishList.WishlistId,
                    WishList = loggedInUser.WishList,
                    FurnitureId = furnitureId,
                    Furniture = furniture
                };
                await _dbContext.AddAsync(newWishlistItem);
                loggedInUser.WishList.WishListDetails.Add(newWishlistItem);
                _dbContext.Update(loggedInUser);
                await _dbContext.SaveChangesAsync();
                return Ok("Add funiture to wishlist succefully");
            }
        }
        //CHECKOUT
        [HttpGet("checkout-now")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> CheckoutNow([Required] string furnitureSpecificationId, [Required] int Quantity)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found ");
            var loggedInUser = await _userManager.FindByEmailAsync(email);
            string checkCustomerInfor = _projectHelper.CheckUserInfor(loggedInUser);
            if (!checkCustomerInfor.IsNullOrEmpty()) return BadRequest(new Response("Error", checkCustomerInfor));

            var orderFurniture = await _dbContext.FurnitureSpecifications.FindAsync(furnitureSpecificationId);
            if (orderFurniture == null) return NotFound($"Furnitur specfication with id = {furnitureSpecificationId} is not exist");
            int available = orderFurniture.FurnitureRepositories == null ? 0 : orderFurniture.FurnitureRepositories.Sum(fr => fr.Available);
            if (available == 0) return StatusCode(StatusCodes.Status403Forbidden,
                new Response("Error", "The furniture specification is out of stock"));

            var deliveryAddress = loggedInUser.UserAddresses.FirstOrDefault(ua => ua.AddressType.Equals("DEFAULT"));
            var response = new
            {
                DeliveryAddressId = deliveryAddress.AddressId,
                DeliveryAddress = deliveryAddress.Address.ToString(),
                FurnitureId = orderFurniture.FurnitureId,
                FurnitureName = orderFurniture.Furniture.FurnitureName,
                FurnitureSpecificationId = furnitureSpecificationId,
                FurnitureSpecificationName = orderFurniture.FurnitureSpecificationName,
                FurnitureSpecificationImage = orderFurniture.Attachments.IsNullOrEmpty() ? "" : _cloudinaryService.GetSharedUrl(orderFurniture.Attachments.First().Path),
                Quantity = Quantity,

                TotalCost = Math.Round(orderFurniture.Price * Quantity, 2)
            };
            return Ok(response);
        }
        //VIA CART
        [HttpGet("checkout-via-cart")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> CheckoutViaCart([Required][FromQuery] List<int> cartIdList)
        {
            if (cartIdList.IsNullOrEmpty()) return BadRequest("cartId is required");
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found ");
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            var checkCustomerInfor = _projectHelper.CheckUserInfor(loggedInUser);
            if (checkCustomerInfor != null) return StatusCode(StatusCodes.Status405MethodNotAllowed,
                new Response("Error", checkCustomerInfor));
            var userCart = loggedInUser.Cart.CartDetails.ToList();
            var selectedItems = new List<CheckoutViewModel>();
            foreach (var id in cartIdList)
            {

                var item = userCart.FirstOrDefault(i => i.CartDetailId == id);
                if (item == null) continue;
                var data = new CheckoutViewModel()
                {
                    FurnitureId = item.FurnitureSpecifition.FurnitureId,
                    FurnitureName = item.FurnitureSpecifition.Furniture.FurnitureName,
                    FurnitureSpecificationId = item.FurnitureSpecificationId,
                    FurnitureSpecificationName = item.FurnitureSpecifition.FurnitureSpecificationName,
                    Quantity = item.Quantity,
                    Cost = Math.Round(item.FurnitureSpecifition.Price * item.Quantity, 2)
                };
                selectedItems.Add(data);
            };

            var deliveryAddress = loggedInUser.UserAddresses.FirstOrDefault(ua => ua.AddressType.Equals("DEFAULT"));
            var response = new
            {
                DeliveryAddressId = deliveryAddress.AddressId,
                DeliveryAddress = deliveryAddress.Address.ToString(),
                items = selectedItems,
                TotalCost = selectedItems.Sum(i => i.Cost),


            };
            return Ok(response);
        }

        //CUSTOMIZE FURNITURE
        [HttpGet("checkout-customize-furniture")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> CheckoutCustomizeFurniture([Required][FromQuery] List<string> customizeFurnitureIdList)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found ");
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            string checkUserInfor = _projectHelper.CheckUserInfor(loggedInUser);
            if (!checkUserInfor.IsNullOrEmpty()) return BadRequest(new Response("Error", checkUserInfor));

            var customizeFurnitureList = loggedInUser.CustomizeFurnitures.ToList();
            var selectedItems = new List<CustomizeFurnitureCheckout>();
            foreach (var id in customizeFurnitureIdList)
            {
                CustomizeFurniture item = customizeFurnitureList.FirstOrDefault(i => i.CustomizeFurnitureId.Equals(id));
                if (item == null) return NotFound($"The customize furniture with id = {id} does not exist in customize furniture list of user");
                if (!item.Result.Status.Equals("Accepted")) return StatusCode(StatusCodes.Status406NotAcceptable,
                    new Response("Error", $"Cannot checkout customize furniture with \"{item.Result.Status}\" status"));
                var data = new CustomizeFurnitureCheckout()
                {

                    CustomizeFurnitureId = item.CustomizeFurnitureId,
                    CustomizeFurnitureName = item.CustomizeFurnitureName,
                    UnitPrice = item.Result.ExpectedPrice.Value,
                    Quantity = item.Quantity,
                    Cost = item.Result.ExpectedPrice.Value * item.Quantity
                };
                selectedItems.Add(data);
            };

            var deliveryAddress = loggedInUser.UserAddresses.FirstOrDefault(ua => ua.AddressType.Equals("DEFAULT"));
            var response = new
            {
                DeliveryAddressId = deliveryAddress.AddressId,
                DeliveryAddress = deliveryAddress.Address.ToString(),
                items = selectedItems,
                TotalCost = selectedItems.Sum(i => i.Cost),


            };
            return Ok(response);
        }

        //ORDER
        [HttpPost("order")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> Order([FromBody] OrderViewModel model)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found ");
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));

            if (loggedInUser.Point < model.UsedPoint) return BadRequest(new Response("Error", "The point of customer is less than the point input"));

            var checkCustomerInfor = _projectHelper.CheckUserInfor(loggedInUser);
            if (checkCustomerInfor != null) return StatusCode(StatusCodes.Status405MethodNotAllowed, new Response("Error", checkCustomerInfor));

            var deliverAddress = await _dbContext.UserAddresses.FirstOrDefaultAsync(a => a.AddressId == model.AddressId && a.UserId.Equals(loggedInUser.Id));
            if (deliverAddress == null) return BadRequest(new Response("Error", $"The address with id = {model.AddressId} is not exist in list of user address"));

            var paymentMethod = await _dbContext.Payments.FindAsync(model.PaymentId);
            if (paymentMethod == null) return BadRequest(new Response("Error", $"The payment with id = {model.PaymentId} does not exist"));


            //define type of order
            bool isCustomizeFurnitureOrder = model.Items.First().ItemId.StartsWith("CF") ? true : false;
            if (isCustomizeFurnitureOrder && model.PaymentId == 1) return BadRequest("Cash payment is not available for custom furniture order");
            var customFurnitureOrderList = new List<CustomizeFurniture>();
            var furnitureOrderList = new List<FurnitureOrderItem>();
            if (isCustomizeFurnitureOrder)
            {
                foreach (var item in model.Items)
                {
                    var customFurniture = await _dbContext.CustomizeFurnitures.FindAsync(item.ItemId);
                    if (customFurniture == null) return BadRequest(new Response("Error", $"Custom furniture with id = {item.ItemId} was not found"));
                    if (customFurniture.Result.Status.Equals("Pending")) return BadRequest(new Response("Error", $"Not allow to order custom furniture with id = {item.ItemId} in status \"Pending\""));
                    customFurnitureOrderList.Add(customFurniture);
                }
            }
            else
            {
                foreach (var item in model.Items)
                {
                    var furnitureSpecification = await _dbContext.FurnitureSpecifications.FindAsync(item.ItemId);
                    if (furnitureSpecification == null) return BadRequest(new Response("Error", $"Furniture specification with id = {item.ItemId} was not found"));
                    var furnitureRepository = furnitureSpecification.FurnitureRepositories.IsNullOrEmpty() ? null : furnitureSpecification.FurnitureRepositories.FirstOrDefault(fr => fr.RepositoryId == 1);
                    int available = furnitureRepository == null ? 0 : furnitureRepository.Available;
                    if (available == 0) return BadRequest($"Cannot order furniture with id = {furnitureSpecification.FurnitureId} because it is out of stock");
                    if (available < item.Quantity) BadRequest($"Cannot order furniture with id = {furnitureSpecification.FurnitureId} because the order quantity is greater than available quantity");
                    var newfurnitureOrderItem = new FurnitureOrderItem()
                    {
                        FurnitureSpecification = furnitureSpecification,
                        FurnitureRepository = furnitureRepository,
                        Quantity = item.Quantity
                    };
                    furnitureOrderList.Add(newfurnitureOrderItem);
                }
            }
            //add new order
            var newOrder = new Order()
            {
                CustomerId = loggedInUser.Id,
                PaymentId = model.PaymentId,
                UsedPoint = model.UsedPoint,
                OrderDate = DateTime.Now,
                DeliveryAddress = deliverAddress.Address.ToString(),
                Note = model.Note == null ? "None" : model.Note,
                Status = "Pending",
                IsPaid = false
            };

            try
            {
                await _dbContext.AddAsync(newOrder);
                await _dbContext.SaveChangesAsync();
                if (model.UsedPoint != 0)
                {
                    loggedInUser.Point -= model.UsedPoint;
                    await _projectHelper.CreatePointHistoryAsync(loggedInUser, -1 * model.UsedPoint, $"Use in order with id = {newOrder.OrderId}");                  
                    _dbContext.Update(loggedInUser);
                    await _dbContext.SaveChangesAsync();
                }

                if (isCustomizeFurnitureOrder)
                {
                    foreach (var item in customFurnitureOrderList)
                    {
                        var newOrderItem = new CustomizeFurnitureOrderDetail()
                        {
                            OrderId = newOrder.OrderId,
                            CustomizeFunitureId = item.CustomizeFurnitureId,
                            Quantity = item.Quantity,
                            Cost = item.Result.ExpectedPrice.Value * item.Quantity
                        };
                        await _dbContext.AddAsync(newOrderItem);
                    }
                    await _dbContext.SaveChangesAsync();
                    newOrder.TotalCost = model.Total / 2;
                    _dbContext.Update(newOrder);
                    await _dbContext.SaveChangesAsync();
                    return Ok(_projectHelper.UrlPayment(newOrder.PaymentId, newOrder.OrderId));
                }
                else
                {
                    //add new order detail 
                    foreach (var item in furnitureOrderList)
                    {
                        var newOrderItem = new FurnitureOrderDetail()
                        {
                            OrderId = newOrder.OrderId,
                            FurnitureSpecificationId = item.FurnitureSpecification.FurnitureSpecificationId,
                            Quantity = item.Quantity,
                            Cost = item.FurnitureSpecification.Price * item.Quantity
                        };
                        //handle repository with furniture repository                      
                        item.FurnitureRepository.Available -= item.Quantity;
                        _dbContext.Update(item.FurnitureRepository);
                        await _dbContext.AddAsync(newOrderItem);
                    }
                    await _dbContext.SaveChangesAsync();
                    newOrder.TotalCost = model.Total;
                    _dbContext.Update(newOrder);
                    await _dbContext.SaveChangesAsync();
                    if (newOrder.PaymentId != 1) return Ok(_projectHelper.UrlPayment(newOrder.PaymentId, newOrder.OrderId));
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response("Error", "An error occurs when processing order"));
            }
            await _projectHelper.CreateAnnouncementAsync(loggedInUser, "Order successfully", "You have  ordered successfully, it will be delivered to you as soon as possible.");
            return Ok("Order successfully");
        }

        [HttpPost("repayment/{orderId}")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> Repayment([FromRoute] int orderId)
        {
            var order = await _dbContext.Orders.FindAsync(orderId);
            if (order == null) return NotFound(new Response("Error", $"The order with id = {orderId} was not found"));
            if (!order.Status.Equals("Pending")) return BadRequest(new Response("Error", $"The order must be in \"Pending\" status to repay"));
            return Ok(_projectHelper.UrlPayment(order.PaymentId, orderId));
        }

        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VNPayReturn()
        {
            var queryString = Request.QueryString;
            if (queryString.HasValue)
            {

                string vnp_HashSecret = _config["VNPAY:HashSecret"];
                var nvc = HttpUtility.ParseQueryString(queryString.ToString());
                var vnpayData = nvc.AllKeys.ToDictionary(k => k, k => nvc[k]);
                VnPayService vnpay = new VnPayService();
                foreach (var vnp in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(vnp.Key) && vnp.Key.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(vnp.Key, vnp.Value);
                    }
                }
                int orderId = Convert.ToInt32(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = vnpayData["vnp_SecureHash"];
                String TerminalID = vnpayData["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100000;
                String bankCode = vnpayData["vnp_BankCode"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    var order = await _dbContext.Orders.FindAsync(orderId);
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        await _projectHelper.CreateAnnouncementAsync(order.Customer, "Order successfully", "You have  ordered successfully, it will be delivered to you as soon as possible.");
                        order.IsPaid = true;
                        order.Status = "Preparing";
                        order.Customer.Spent += order.TotalCost;
                        if (!order.CustomizeFurnitureOrderDetails.IsNullOrEmpty())
                        {
                            order.Customer.Debit += order.TotalCost;
                            order.TotalCost *= 2;
                            order.IsPaid = false;
                            order.Note += " (deposit has been paid)";
                        }
                        _dbContext.UpdateRange(order, order.Customer);
                        await _dbContext.SaveChangesAsync();
                        return RedirectPermanent("http://localhost:8080/ProfileCusPage");
                        //return Ok($"Payment success OrderId={orderId}, VNPAY TranId={vnpayTranId}");
                    }
                    else
                    {
                        await _projectHelper.CreateAnnouncementAsync(order.Customer, "Failed to order", "Order failed due to an error during the payment process. Please pay again.");
                        return RedirectPermanent("http://localhost:8080/ProfileCusPage");
                        //return StatusCode(StatusCodes.Status406NotAcceptable,
                        //    new Response("error", $"error payment OrderId={orderId}"));
                    }
                }
                else
                {
                    return BadRequest("An error occurred during processing");
                }
            }
            return BadRequest("Return data not found");
        }

        [HttpPut("cancel-order")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {

            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found ");
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            Order order = loggedInUser.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null) return NotFound($"The order with Id = {orderId} does not exist");
            if (!order.Status.Equals("Preparing") && !order.Status.Equals("Pending")) return BadRequest($"Cannot cancel the order with \"{order.Status}\" status");

            //refund if user pays order via VNPAY
            if (order.PaymentId != 1)
            {
                if (!order.CustomizeFurnitureOrderDetails.IsNullOrEmpty())
                {
                    order.Customer.Debit -= order.TotalCost / 2;
                    order.Customer.Spent -= order.TotalCost / 2;
                }

                if (!order.FurnitureOrderDetails.IsNullOrEmpty())
                {
                    if (order.IsPaid)
                    {
                        order.Customer.Spent -= order.TotalCost;
                    }
                    goto handle;
                }
                var result = _projectHelper.Refund(order);
                if (!result) return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", "An error occur during refund process"));
            }
        handle:
            //change status
            order.Status = "Canceled";
            //restore quantity of available
            if (!order.FurnitureOrderDetails.IsNullOrEmpty())
            {
                foreach (var orderItem in order.FurnitureOrderDetails)
                {
                    var furnitureRepository = await _dbContext.FurnitureRepositories.FirstOrDefaultAsync(fr => fr.RepositoryId == 1 && fr.FurnitureSpecificationId == orderItem.FurnitureSpecificationId);
                    furnitureRepository.Available += orderItem.Quantity;
                    _dbContext.Update(furnitureRepository);
                }
            }
            await _projectHelper.CreateAnnouncementAsync(loggedInUser, "Success order cancelation", "Order cancellation is successful, payment fee will be refunded to your bank account if customer pays order via internet banking");
            _dbContext.UpdateRange(order, order.Customer);
            await _dbContext.SaveChangesAsync();
            return Ok("Cancellation is successful, payment fee will be refunded to your bank account if customer pays order via internet banking");
        }

        [HttpGet("get-order")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetOrder(string status)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found ");
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            var orderList = new List<Order>();
            if (status.Equals("All")) orderList = loggedInUser.Orders.ToList();
            else orderList = loggedInUser.Orders.Where(o => o.Status.Equals(status)).ToList();
            if (orderList.Count == 0) return Ok(orderList);
            orderList = orderList.OrderByDescending(o => o.OrderDate).ToList();
            var response = new List<object>();
            foreach (var order in orderList)
            {
                if (order.TotalCost > 0)
                {
                    bool isValid = false;
                    var isCustomOrder = order.CustomizeFurnitureOrderDetails.IsNullOrEmpty() ? false : true;
                    if (isCustomOrder)
                    {
                        isValid = order.Status.Equals("Pending") && order.OrderDate.AddMinutes(10) < DateTime.Now  ? false : true;
                    }
                    else
                    {
                        isValid = (order.Status.Equals("Pending") && order.Payment.PaymentId != 1 && DateTime.Now > order.OrderDate.AddMinutes(10)) ? false : true;
                    }

                    if (!isValid)
                    {
                        order.Status = "Canceled";
                        _dbContext.Update(order);
                        await _projectHelper.CreateAnnouncementAsync(order.Customer, "Order canceled", $"Your order \"{order.CustomerId}\" has been canceled because of overdue payment");
                        await _dbContext.SaveChangesAsync();
                    }


                    var furnitures = new object();
                    if (!order.FurnitureOrderDetails.IsNullOrEmpty())
                    {
                        furnitures = order.FurnitureOrderDetails.Select(o => new
                        {
                            FurnitureId = o.FurnitureSpecification.Furniture.FurnitureId,
                            FurnitureName = o.FurnitureSpecification.Furniture.FurnitureName,
                            FurnitureSpecificationId = o.FurnitureSpecification.FurnitureSpecificationId,
                            FurnitureSpecificationname = o.FurnitureSpecification.FurnitureSpecificationName,
                            Quantity = o.Quantity,
                            Cost = o.Cost,
                        });
                    }
                    else
                    {
                        furnitures = order.CustomizeFurnitureOrderDetails.Select(o => new
                        {
                            CustomFurniture = o.CustomizeFunitureId,
                            CustomFurnitureName = o.CustomizeFurniture.CustomizeFurnitureName,
                            Quantity = o.Quantity,
                            Cost = o.Cost,
                        });
                    }


                    var data = new
                    {
                        OrderId = order.OrderId,
                        DeliveryAddress = order.DeliveryAddress,
                        Furniture = furnitures,
                        PaymentMethod = order.Payment.PaymentMethod,
                        TotalCost = order.CustomizeFurnitureOrderDetails.IsNullOrEmpty() ? order.TotalCost : order.TotalCost,
                        OrderDate = order.OrderDate,
                        Status = order.Status,
                        Note = order.Note,
                        IsPaid = order.IsPaid,
                        isCustomOrder = order.CustomizeFurnitureOrderDetails.IsNullOrEmpty() ? false : true,

                    };
                    response.Add(data);
                }
            }
            return Ok(response);
        }

        //FEEDBACK
        [HttpGet("feedbacks")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetUserFeedbacks()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found ");
            var loggedInUser = await _userManager.FindByEmailAsync(email);
            if (loggedInUser.Feedbacks.IsNullOrEmpty()) return Ok(new List<Feedback>());

            var responses = loggedInUser.Feedbacks.OrderByDescending(f => f.CreationDate).Select(fb => new
            {
                FeedbackId = fb.FeedbackId,
                OrderId = fb.OrderId,
                FurnitureId = fb.FurnitureSpecification.Furniture.FurnitureId,
                FurnitureName = fb.FurnitureSpecification.Furniture.FurnitureName,
                FeedbackImages = fb.Attachements.Count > 0 ? fb.Attachements.Select( a => new
                {
                    url =  _cloudinaryService.GetSharedUrl(a.Path),
                    type = a.Type
                }) : null,
                Content = fb.Content,
                VoteStar = fb.VoteStar
            });
            return Ok(responses);
        }

        [HttpPost("create-feedback")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> CreateFeedback([FromForm] FeedbackViewModel model)
        {
            //validation
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound(new Response("Error", "Logged in user not found"));
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));

            var order = await _dbContext.Orders.FindAsync(model.OrderId);
            if (order == null) return NotFound("The order does not exist");
            if (!order.Status.Equals("Delivered")) return StatusCode(StatusCodes.Status406NotAcceptable,
                new Response("Error", "The order must be in \"Delivered\" status to give feedback"));

            var furnitureOrderDetailExist = order.FurnitureOrderDetails.FirstOrDefault(fod => fod.FurnitureSpecificationId.Equals(model.FurnitureSpecificationId));
            if (furnitureOrderDetailExist == null) return NotFound($"The order does not contains furniture specification with id = {model.FurnitureSpecificationId}");
            var specification = furnitureOrderDetailExist.FurnitureSpecification;
            var specificationFeedbacks = specification.Feedbacks.ToList();
            if (specificationFeedbacks.FirstOrDefault(fb => fb.OrderId == order.OrderId) != null)
                return StatusCode(StatusCodes.Status406NotAcceptable,
               new Response("Error", "This furniture has received feedback"));


            //create new feedback 
            var newFeedback = new Feedback()
            {
                CustomerId = loggedInUser.Id,
                OrderId = order.OrderId,
                FurnitureSpecificationId = model.FurnitureSpecificationId,
                Content = _projectHelper.FilterBadWords(model.Content),
                VoteStar = model.VoteStar,
                Anonymous = model.Anonymous,
                CreationDate = DateTime.Now
            };

            //add point to customer point 
            loggedInUser.Point += 200;
            _projectHelper.CreatePointHistoryAsync(loggedInUser, 200, $"Give feedback {specification.Furniture.FurnitureName} successfully");
            //PointHistory newPointHistory = new PointHistory()
            //{
            //    CustomerId = loggedInUser.Id,
            //    Description = $"Give feedback {specification.Furniture.FurnitureName} successfully +200 point",
            //    History = DateTime.Now
            //};

            //calculate average vote star of furniture and update 
            var feedbackList = await _dbContext.Feedbacks.Where(fb => fb.FurnitureSpecification.Furniture.FurnitureId == specification.Furniture.FurnitureId).ToListAsync();
            double sumStar = newFeedback.VoteStar;
            if (feedbackList.Count > 0)
            {
                foreach (var feedback in feedbackList)
                {
                    sumStar += feedback.VoteStar;
                }
            }
            Furniture furniture = await _dbContext.Furnitures.FindAsync(specification.FurnitureId);
            var averageStar = Math.Round(sumStar / (feedbackList.Count + 1), 1);
            furniture.VoteStar = averageStar;
            try
            {
                await _dbContext.AddAsync(newFeedback);
                _dbContext.Update(furniture);
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
              new Response("Error", "An error occur when creating feedback"));
            }


            //upload image or video
            if (!model.files.IsNullOrEmpty())
            {
                var addedFeedback = await _dbContext.Feedbacks.FirstOrDefaultAsync(fb => fb.OrderId == order.OrderId &&
                                           fb.FurnitureSpecificationId == specification.FurnitureSpecificationId);
                foreach (var file in model.files)
                {
                    var newFeedbackAttachment = new FeedbackAttachment()
                    {
                        FeedbackId = addedFeedback.FeedbackId,
                        AttachmentName = file.FileName,
                        Path = _cloudinaryService.UploadFile(file),
                        Type = _cloudinaryService.ImageOrVideo(file)
                    };
                    await _dbContext.AddAsync(newFeedbackAttachment);
                }

            }
            try
            {

                await _dbContext.SaveChangesAsync();
                return Ok("Create feedback successfully");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
               new Response("Error", "An error occur when uploading image "));
            }

        }
        [HttpPut("edit-feedback/{feedbackId}")]
        public async Task<IActionResult> UpdateFeedback([FromRoute] int feedbackId, [FromForm] EditFeedbackViewModel model)
        {
            //validation
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound(new Response("Error", "Logged in user not found"));
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));

            Feedback feedbackExist = await _dbContext.Feedbacks.FindAsync(feedbackId);
            if (feedbackExist == null) return NotFound(new Response("Error", $"The feedback with id = {feedbackExist} was not found"));

            //edit new feedback 
            feedbackExist.Content = _projectHelper.FilterBadWords(model.Content);
            feedbackExist.VoteStar = model.VoteStar;
            feedbackExist.Anonymous = model.Anonymous;
            feedbackExist.CreationDate = DateTime.Now;


            //calculate average vote star of furniture and update 
            var feedbackList = await _dbContext.Feedbacks.Where(fb => fb.FurnitureSpecification.Furniture.FurnitureId == feedbackExist.FurnitureSpecification.Furniture.FurnitureId).ToListAsync();
            double sumStar = feedbackExist.VoteStar;
            if (feedbackList.Count > 0)
            {
                foreach (var feedback in feedbackList)
                {
                    sumStar += feedback.VoteStar;
                }
            }
            Furniture furniture = await _dbContext.Furnitures.FindAsync(feedbackExist.FurnitureSpecification.FurnitureId);
            var averageStar = Math.Round(sumStar / (feedbackList.Count + 1), 1);
            furniture.VoteStar = averageStar;
            try
            {
                _dbContext.UpdateRange(furniture, feedbackExist);
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
              new Response("Error", "An error occur when creating feedback"));
            }


            //upload image or video

            if (!model.files.IsNullOrEmpty())
            {
                if (!feedbackExist.Attachements.IsNullOrEmpty())
                {
                    foreach (var attachment in feedbackExist.Attachements)
                    {
                        _cloudinaryService.RemoveFile(attachment.Path);
                        _dbContext.Remove(attachment);
                    }
                    await _dbContext.SaveChangesAsync();
                }

                foreach (var file in model.files)
                {
                    var newFeedbackAttachment = new FeedbackAttachment()
                    {
                        FeedbackId = feedbackExist.FeedbackId,
                        AttachmentName = file.FileName,
                        Path =  _cloudinaryService.UploadFile(file),
                        Type = _cloudinaryService.ImageOrVideo(file)
                    };
                    await _dbContext.AddAsync(newFeedbackAttachment);
                }

            }
            try
            {

                await _dbContext.SaveChangesAsync();
                return Ok(new Response("Success", "Update feedback successfully"));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
               new Response("Error", "An error occur when uploading image "));
            }
        }
        //CUSTOMIZE FURNITURE


        //GET
        [HttpGet("customize-furnitures")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetCustomizeFurniture(string status)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in in user not found ");
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            List<CustomizeFurniture> customizeFurnitures = new List<CustomizeFurniture>();
            if (status.Equals("All")) customizeFurnitures = loggedInUser.CustomizeFurnitures.ToList();
            else customizeFurnitures = loggedInUser.CustomizeFurnitures.OrderByDescending(cf => cf.CreationDate).Where(cf => cf.Result.Status.Equals(status)).ToList();
            if (customizeFurnitures.Count == 0) return NotFound($"There is no customize furniture with {status} status");
            try
            {
                var response = customizeFurnitures.Select(cf => new
                {
                    CustomizeFurnitureId = cf.CustomizeFurnitureId,
                    CustomerId = cf.CustomerId,
                    CustomizeFurnitureName = cf.CustomizeFurnitureName,
                    ColorId = cf.ColorId,
                    Color = cf.Color.ColorName,
                    Height = cf.Height,
                    Width = cf.Width,
                    Length = cf.Length,
                    WoodId = cf.WoodId,
                    Wood = cf.Wood.WoodType,
                    Quantity = cf.Quantity,
                    DesiredCompletionDate = cf.DesiredCompletionDate,
                    CreationDate = cf.CreationDate,
                    Images = cf.Attachments.Where(a => a.Type.Equals("images")).Select( a => new
                    {
                        AttachmentName = a.AttachmentName,
                        Path = _cloudinaryService.GetSharedUrl(a.Path)
                    }),
                    Videos = cf.Attachments.Where(a => a.Type.Equals("videos")).Select( a => new
                    {
                        AttachmentName = a.AttachmentName,
                        Path =  _cloudinaryService.GetSharedUrl(a.Path)
                    }),
                    Result = new
                    {
                        Status = cf.Result.Status,
                        ExpectedPrice = cf.Result.ExpectedPrice,
                        ActualCompletionDate = cf.Result.ActualCompletionDate,
                        Reason = cf.Result.Reason
                    }

                }).ToList();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", "An error occurs during fetch data"));
            }

        }

        //CREATE
        [HttpPost("customize-furnitures/create")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> CreateCustomizeFurniture([FromForm] CustomizeFurnitureViewModel userInput)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found ");
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            //var checkCustomerInfor = CheckCustomerInfor(loggedInUser);
            //if (checkCustomerInfor != null) return StatusCode(StatusCodes.Status405MethodNotAllowed,
            //    new Response() { Status = "Error", Message = checkCustomerInfor });

            // tao customize furniture de shop owner duyet
            var newCustomizeFurniture = new CustomizeFurniture()
            {
                CustomizeFurnitureId = "CF-" + Guid.NewGuid().ToString().ToUpper(),
                CustomerId = loggedInUser.Id,
                CustomizeFurnitureName = userInput.CustomizeFurnitureName,
                CategoryId = userInput.CategoryId,
                ColorId = userInput.ColorId,
                Height = userInput.Height,
                Width = userInput.Width,
                Length = userInput.Length,
                WoodId = userInput.WoodId,
                Quantity = userInput.Quantity,
                Description = _projectHelper.FilterBadWords(userInput.Description),
                DesiredCompletionDate = userInput.DesiredCompletionDate,
                CreationDate = DateTime.Now
            };
            await _dbContext.AddAsync(newCustomizeFurniture);
            await _dbContext.SaveChangesAsync();

            var newResult = new Result()
            {
                CustomizeFurnitureId = newCustomizeFurniture.CustomizeFurnitureId,
                Status = "Pending"
            };
            await _dbContext.AddAsync(newResult);
            await _dbContext.SaveChangesAsync();
            if (!userInput.Attachments.IsNullOrEmpty())
            {
                foreach (var file in userInput.Attachments)
                {
                    var newAttachment = new CustomizeFurnitureAttachment()
                    {
                        CustomizeFurnitureId = newCustomizeFurniture.CustomizeFurnitureId,
                        AttachmentName = file.FileName,
                        Path = _cloudinaryService.UploadFile(file),
                        Type = _cloudinaryService.ImageOrVideo(file)
                    };
                    await _dbContext.AddAsync(newAttachment);
                }
                await _dbContext.SaveChangesAsync();
            }
            await _dbContext.SaveChangesAsync();
            return Ok("Create customize furniture successfully");
        }

        //UPDATE
        [HttpPut("customize-furnitures/update")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> UpdateCustomizeFurniture([FromForm] EditCustomizeFurnitureViewModel userInput)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found ");
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            CustomizeFurniture customizeFurniture = loggedInUser.CustomizeFurnitures.FirstOrDefault(cf => cf.CustomizeFurnitureId.Equals(userInput.CustomizeFurnitureId));
            if (customizeFurniture == null) return NotFound("Customize furniture not found");
            if (!customizeFurniture.Result.Status.Equals("Pending")) return StatusCode(StatusCodes.Status406NotAcceptable,
                new Response("Error", "The customize furniture can only be changed while in the pending status"));

            customizeFurniture.CustomizeFurnitureName = userInput.CustomizeFurnitureName.IsNullOrEmpty() ? customizeFurniture.CustomizeFurnitureName : userInput.CustomizeFurnitureName;
            customizeFurniture.ColorId = !userInput.ColorId.HasValue || userInput.ColorId.Value == 0 ? customizeFurniture.ColorId : userInput.ColorId.Value;
            customizeFurniture.Height = !userInput.Height.HasValue || userInput.Height.Value == 0 ? customizeFurniture.Height : userInput.Height.Value;
            customizeFurniture.Width = !userInput.Width.HasValue || userInput.Width.Value == 0 ? customizeFurniture.Width : userInput.Width.Value;
            customizeFurniture.Length = !userInput.Length.HasValue || userInput.Length.Value == 0 ? customizeFurniture.Length : userInput.Length.Value;
            customizeFurniture.WoodId = !userInput.WoodId.HasValue || userInput.WoodId.Value == 0 ? customizeFurniture.WoodId : userInput.WoodId.Value;
            customizeFurniture.Quantity = !userInput.Quantity.HasValue || userInput.Quantity.Value == 0 ? customizeFurniture.Quantity : userInput.Quantity.Value;
            customizeFurniture.DesiredCompletionDate = !userInput.DesiredCompletionDate.HasValue ? customizeFurniture.DesiredCompletionDate : userInput.DesiredCompletionDate.Value;
            _dbContext.Update(customizeFurniture);
            await _dbContext.SaveChangesAsync();
            if (!userInput.Attachments.IsNullOrEmpty())
            {
                foreach (var attachment in customizeFurniture.Attachments)
                {
                    bool removeResult = _cloudinaryService.RemoveFile(attachment.Path);
                    if (removeResult) _dbContext.Remove(attachment);
                };
                await _dbContext.SaveChangesAsync();
                foreach (var file in userInput.Attachments)
                {

                    var newAttachment = new CustomizeFurnitureAttachment()
                    {
                        CustomizeFurnitureId = customizeFurniture.CustomizeFurnitureId,
                        AttachmentName = file.FileName,
                        Path =  _cloudinaryService.UploadFile(file),
                        Type = _cloudinaryService.ImageOrVideo(file)
                    };
                    await _dbContext.AddAsync(newAttachment);
                }
                await _dbContext.SaveChangesAsync();
            }
            return Ok("Update customize furniture successfully");
        }

        //DELETE
        [HttpDelete("customize-furnitures/remove")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> DeleteCustomizeFurniture(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found ");
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            CustomizeFurniture customizeFurniture = await _dbContext.CustomizeFurnitures.FindAsync(id);
            if (customizeFurniture == null) return NotFound("Customize furniture not found");
            try
            {
                if (!customizeFurniture.Attachments.IsNullOrEmpty())
                {
                    _dbContext.RemoveRange(customizeFurniture.Attachments);
                    await _dbContext.SaveChangesAsync();
                }
                if (customizeFurniture.Result != null)
                {
                    _dbContext.RemoveRange(customizeFurniture.Result);

                }
                _dbContext.Remove(customizeFurniture);
                await _dbContext.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", $"An error occurred while removing custom furniture {ex.Message}"));

            }
        }

        //WARRANTY

        //GET
        [HttpGet("warranties")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetWarranties(string? status)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found ");
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            status = status.IsNullOrEmpty() ? "All" : status;
            if (!status.Equals("Pending") && !status.Equals("Accepted") && !status.Equals("Not accepted") && !status.Equals("All")) return BadRequest("The status must be \"Pending\", \"Accepted\", \"Not accepted\" and \"All\"");
            var warrantyList = loggedInUser.Warranties.ToList();
            if (!status.Equals("All")) warrantyList = warrantyList.Where(w => w.Status.Equals(status)).ToList();
            if (warrantyList.Count == 0) return NotFound("The customer has no any warranty claim");
            var respones = warrantyList.Select(w => new
            {
                WarrantyId = w.WarrantyId,
                OrderId = w.OrderId,
                WarrantyReason = w.WarrantyReasons,
                EstimatedTime = w.EstimatedTime.HasValue ? String.Format("{0:yyyy/MM/dd}", w.EstimatedTime) : "Processing",
                Attacments = new
                {
                    Images = w.Attachments.Where(a => a.Type.Equals("images")).Select( a => new
                    {
                        FileName = a.AttachmentName,
                        Path = _cloudinaryService.GetSharedUrl(a.Path)
                    }),
                    Videos = w.Attachments.Where(a => a.Type.Equals("videos")).Select( a => new
                    {
                        FileName = a.AttachmentName,
                        Path = _cloudinaryService.GetSharedUrl(a.Path)
                    })
                },
                Status = w.Status
            }).ToList();
            return Ok(respones);
        }

        //CREATE
        [HttpPost("warranties/create")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> CreateWarranty([FromForm] WarrantyViewModel userInput)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found ");
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            var orderExist = loggedInUser.Orders.FirstOrDefault(w => w.OrderId == userInput.OrderId);
            if (orderExist == null) return NotFound($"The order witd id = {userInput.OrderId} does not exist in the ordered list of customer");
            if (!orderExist.Status.Equals("Delivered")) return BadRequest($"Not allow to create warranty when order is in {orderExist.Status} status");

            Warranty newWarranty = new Warranty()
            {
                UserId = loggedInUser.Id,
                OrderId = userInput.OrderId,
                WarrantyReasons = userInput.WarrantyReasons,
                Status = "Pending"
            };
            try
            {
                await _dbContext.AddAsync(newWarranty);
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", "An error occur when creating new warranty"));
            }

            if (!userInput.UploadFiles.IsNullOrEmpty())
            {
                foreach (var file in userInput.UploadFiles)
                {

                    var newAttachment = new WarrantyAttachment()
                    {
                        WarrantyId = newWarranty.WarrantyId,
                        AttachmentName = file.FileName,
                        Path = _cloudinaryService.UploadFile(file),
                        Type = _cloudinaryService.ImageOrVideo(file)
                    };
                    await _dbContext.AddAsync(newAttachment);
                }
                await _dbContext.SaveChangesAsync();
            }
            return StatusCode(StatusCodes.Status201Created,
                new Response("Success", "Create warranty successfully"));
        }

        //UPDATE
        [HttpPut("warranties/update")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> UpdateWarranty([FromForm] EditWarrantyViewModel userInput)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found ");
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            var editWarranty = loggedInUser.Warranties.FirstOrDefault(w => w.WarrantyId == userInput.WarrantyId);
            if (editWarranty == null) return NotFound($"warranty claim with id = {userInput.WarrantyId} was not found in in the customer's warranty claim list");

            editWarranty.WarrantyReasons = userInput.WarrantyReasons;

            _dbContext.Update(editWarranty);
            await _dbContext.SaveChangesAsync();
            if (!userInput.UploadFiles.IsNullOrEmpty())
            {
                if (!editWarranty.Attachments.IsNullOrEmpty())
                {
                    foreach (var attachment in editWarranty.Attachments)
                    {
                        var removeResult =  _cloudinaryService.RemoveFile(attachment.Path);
                        if (removeResult) _dbContext.Remove(attachment);
                    };
                }

                foreach (var file in userInput.UploadFiles)
                {
                    var newAttachment = new WarrantyAttachment()
                    {
                        WarrantyId = editWarranty.WarrantyId,
                        AttachmentName = file.FileName,
                        Path = _cloudinaryService.UploadFile(file),
                        Type = _cloudinaryService.ImageOrVideo(file)
                    };
                    _dbContext.Update(newAttachment);
                }
                await _dbContext.SaveChangesAsync();
            }
            return StatusCode(StatusCodes.Status201Created,
                new Response("Success", "Edit warranty successfully"));
        }

        //DELETE
        [HttpDelete("warranties/remove/{id}")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> RemoveWarranty([Required] int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found ");
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            Warranty removeWarranty = loggedInUser.Warranties.FirstOrDefault(w => w.WarrantyId == id);
            if (removeWarranty == null) return NotFound($"warranty claim with id = {id} was not found in in the customer's warranty claim list");

            try
            {
                if (!removeWarranty.Attachments.IsNullOrEmpty())
                {
                    foreach (var attachment in removeWarranty.Attachments)
                    {
                        var removeResult = _cloudinaryService.RemoveFile(attachment.Path);
                        _dbContext.Remove(attachment);
                    };
                    removeWarranty.Attachments.Clear();
                }
                _dbContext.Remove(removeWarranty);
                await _dbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status204NoContent,
                    new Response("Success", "Remove warranty claim successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   new Response("Error", "An error occurs when removing warranty claim"));
            }

        }
    }
}
