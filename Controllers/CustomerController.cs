using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Data;
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

namespace OnlineShopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private readonly ISMSService _smsService;
        private readonly IFirebaseService _firebaseService;
        private readonly IProjectHelper _projectHelper;
        public CustomerController(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> userRole,
            SignInManager<User> signInManager, IConfiguration config, IEmailService emailService, ISMSService smsService, IFirebaseService firebaseService, IProjectHelper projectHelper)
        {
            _dbContext = context;
            _userManager = userManager;
            _roleManager = userRole;
            _signInManager = signInManager;
            _config = config;
            _emailService = emailService;
            _smsService = smsService;
            _firebaseService = firebaseService;
            _projectHelper = projectHelper;
        }

                                                   

        //FURNITURE
        [HttpGet("furnitures")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllFurniture() 
        {         
            var furnitures = await _dbContext.Furnitures.ToListAsync();
            var response = furnitures.Select(f => new
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
                        _firebaseService.GetDownloadUrl(f.FurnitureSpecifications.FirstOrDefault().Attachments.FirstOrDefault().Path) 
                        : String.Empty,
                CollectionId = f.CollectionId.HasValue ? f.CollectionId.Value.ToString() : "None",
                Collection = f.Collection.CollectionName
            }) ;           

            return Ok(response);
        }


        [HttpGet("furnitures/{id}")]
        [AllowAnonymous] 
        public async Task<IActionResult> GetFurnitureSpecificationById(int id) //thieu available
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
                    Images = fs.Attachments.Where(a => a.Type.Equals("images")).Select(a => new
                    {
                        AttachmentName = a.AttachmentName,
                        Path = _firebaseService.GetDownloadUrl(a.Path)
                    }),
                    Videos = fs.Attachments.Where(a => a.Type.Equals("vfeideos")).Select(a => new
                    {
                        AttachmentName = a.AttachmentName,
                        Path = _firebaseService.GetDownloadUrl(a.Path)
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
        [AllowAnonymous] 
        public async Task<IActionResult> SearchFurnitureF(string keyword)
        {
            if (!keyword.IsNullOrEmpty()) keyword = keyword.ToUpper();
            var result = await _dbContext.Furnitures.Where(f => f.FurnitureName.ToUpper().Contains(keyword)).ToListAsync();
            if (result.IsNullOrEmpty()) return Ok(new List<Furniture>());
            var response = result.Select(f => new
            {
                FurnitureId = f.FurnitureId,
                FurnitureName = f.FurnitureName,
                VoteStar = f.VoteStar,
                Sold = f.Sold,
                Label = f.Label.LabelName,
                Available = f.FurnitureSpecifications.IsNullOrEmpty() ? 0 : f.FurnitureSpecifications.Sum(fs => fs.FurnitureRepositories.Sum(fr => fr.Available)),
                Price = f.FurnitureSpecifications.Max(fs => fs.Price) == f.FurnitureSpecifications.Min(fs => fs.Price)
                          ? $"{f.FurnitureSpecifications.Max(fs => fs.Price)}" : $"{f.FurnitureSpecifications.Min(fs => fs.Price)} - {f.FurnitureSpecifications.Max(fs => fs.Price)}",
                Image = f.FurnitureSpecifications.FirstOrDefault().Attachments.IsNullOrEmpty() ? String.Empty :
                        _firebaseService.GetDownloadUrl(f.FurnitureSpecifications.FirstOrDefault().Attachments.FirstOrDefault().Path)
                        
            });
            return Ok(response);
        }
        
        [HttpGet("furnitures/filter")] 
        [AllowAnonymous]
        public async Task<IActionResult> FurnitureFilter([FromQuery] FurnitureFilterViewModel filter)
        {
            filter.MaxCost = filter.MaxCost == 0 ? double.MaxValue : filter.MaxCost;
            var funituresSpecifications = await _dbContext.FurnitureSpecifications.Where(fs => fs.Price >= filter.MinCost && fs.Price <= filter.MaxCost).ToListAsync();
            if (filter.Category != null) funituresSpecifications = funituresSpecifications.Where(fs => fs.Furniture.Category.Equals(filter.Category)).ToList();
            if (filter.Star != 0) funituresSpecifications = funituresSpecifications.Where(fs => fs.Furniture.VoteStar > filter.Star && fs.Furniture.VoteStar < filter.Star + 1).ToList();
            if (filter.AppropriateRoom != null) funituresSpecifications = funituresSpecifications.Where(fs => fs.Furniture.AppopriateRoom.Equals(filter.AppropriateRoom)).ToList();
            if (filter.Collection != null) funituresSpecifications = funituresSpecifications.Where(fs => fs.Furniture.Collection.Equals(filter.Collection)).ToList();
            var response = funituresSpecifications.GroupBy(fs => fs.Furniture).Select(gr => new
            {
                FurnitureId = gr.Key.FurnitureId,
                FurnitureName = gr.Key.FurnitureName,
                VoteStar = gr.Key.VoteStar,
                Sold = gr.Key.Sold,
                Label = gr.Key.Label.LabelName,
                Image = _firebaseService.GetDownloadUrl(gr.FirstOrDefault().Attachments.FirstOrDefault().Path),
                Price = gr.Min(fs => fs.Price) == gr.Max(fs => fs.Price) ? $"{gr.Max(fs => fs.Price)}" : $"{gr.Min(fs => fs.Price)}- {gr.Max(fs => fs.Price)}",
            }).ToList();
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
            if (email == null) return NotFound("Loggined user not found ");
            var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            var furnitureSpecification = await _dbContext.FurnitureSpecifications.FindAsync(furnitureSpecificationId);
            if (furnitureSpecification == null) return NotFound("The furniture not found");
           
            //kiem tra san pham da co trong gio hang chua
            var cartDetailExist = await _dbContext.CartDetails.FirstOrDefaultAsync(cd => cd.CartId == loggedInUser.Cart.CartId
                                           && cd.FurnitureSpecificationId.Equals(furnitureSpecificationId));
            if (cartDetailExist != null)
            {
                if (quantity != 0)
                {
                    cartDetailExist.Quantity += quantity;
                    _dbContext.Update(cartDetailExist);
                }             
            }
            else
            {
                if (quantity == 0)
                {
                    return BadRequest("Cannot add the furniture with zero quantity to cart");
                }
                var newCartDetail = new CartDetail()
                {   CartId =   loggedInUser.Cart.CartId,                              
                    FurnitureSpecificationId = furnitureSpecificationId,
                    Quantity = quantity,                   
                };
                await _dbContext.AddAsync(newCartDetail);
                loggedInUser.Cart.CartDetails.Add(newCartDetail);
            }
            await _dbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status200OK,
                   new Response("Success", "Add the furniture to cart successfully"));
        }

        [HttpDelete("cart/remove/{id}")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> DeleteCartItem(string id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            var cartItems = loggedInUser.Cart.CartDetails.ToList();
            var deleteItem = cartItems.FirstOrDefault(c => c.FurnitureSpecificationId.Equals(id));
            if (deleteItem == null) return NotFound("This furniture is not in the cart");
            try
            {
                _dbContext.Remove(deleteItem);
                await _dbContext.SaveChangesAsync();
                return Ok("Remove furniture from cart successfully");
            } catch (Exception ex)
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
            if (email == null) return NotFound("Loggined user not found ");
            var user = await _userManager.FindByEmailAsync(email);
            var announcement = user.Announcements.OrderByDescending(a => a.CreationDate).Take(10).Select(a => new
            {
                Title = a.Title,
                Content = a.Content,
                Date = a.CreationDate

            }).ToList();
            return Ok(announcement);
        }

                                                                              //WISHLIST

        [HttpGet("wish-list")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetWishlist()
        {


            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return Unauthorized(new Response("Error","Logged in user not found "));
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
            var response = wishList.Select(w => new
            {
                FurnitureId = w.Furniture.FurnitureId,
                FurnitureName = w.Furniture.FurnitureName,
                VoteStar = w.Furniture.VoteStar,
                Sold = w.Furniture.Sold,
                Label = w.Furniture.Label.LabelName,
                Available = w.Furniture.FurnitureSpecifications.IsNullOrEmpty() ? 0 : w.Furniture.FurnitureSpecifications.Sum(fs => fs.FurnitureRepositories.Sum(fr => fr.Available)),
                Price = w.Furniture.FurnitureSpecifications.Max(fs => fs.Price) == w.Furniture.FurnitureSpecifications.Min(fs => fs.Price)
                           ? $"{w.Furniture.FurnitureSpecifications.Max(fs => fs.Price)}" : $"{w.Furniture.FurnitureSpecifications.Min(fs => fs.Price)} - {w.Furniture.FurnitureSpecifications.Max(fs => fs.Price)}",
                Image = w.Furniture.FurnitureSpecifications.FirstOrDefault().Attachments.IsNullOrEmpty() ? String.Empty :
                        _firebaseService.GetDownloadUrl(w.Furniture.FurnitureSpecifications.FirstOrDefault().Attachments.FirstOrDefault().Path) 
            }).ToList();
            return Ok(response);
        }


        [HttpPut("wish-list/toggle")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> ToggleWishlist(int furnitureId)
        
        {
            if (furnitureId == null) return BadRequest();
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
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

        //                                                                   //PHONE NUMBER
        ////VIEW
        //[HttpGet("customer-infor/phone-number")]
        //[Authorize(Roles = "CUSTOMER")]
        //public async Task<IActionResult> GetCustomerPhoneNum()
        //{
        //    var email = User.FindFirstValue(ClaimTypes.Email);
        //    if (email == null) return NotFound("Loggined user not found ");
        //    var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
        //    if (loggedInUser.PhoneNumber == null) return NotFound("User has no phonenumber added"); // user login via Google usually lack of phoneNum
        //    var response = new
        //    {
        //        PhoneNumber = loggedInUser.PhoneNumber,
        //        Confirm = loggedInUser.PhoneNumberConfirmed
        //     };
        //    return Ok(response);  
        //}
  
        //[HttpGet("customer-infor/phone-number/get-otp")] 
        //public async Task<IActionResult> SendOTPConfirmation(string phoneNums)
        //{   var phoneExist = await _dbContext.Users.Where(u => u.PhoneNumber.Equals(phoneNums) && u.PhoneNumberConfirmed == true).ToListAsync();
        //    if (phoneExist.Count > 0) return StatusCode(StatusCodes.Status406NotAcceptable, 
        //        new Response("Error", "This phone number is already linked to another account"));
        //    var email = User.FindFirstValue(ClaimTypes.Email);
        //    if (email == null) return NotFound("Loggined user not found ");
        //    var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
        //    byte[] secretKey = Encoding.ASCII.GetBytes(phoneNums);
        //    var otp = new Totp(secretKey, step: 300, totpSize: 6);
        //    var totpCode = otp.ComputeTotp(DateTime.Now);
        //    var sms = new Sms(new string[] { phoneNums }, $"The OTP to confirm your phone number: {totpCode}");
        //    _smsService.SendSMS(sms);
        //    return Ok($"The confirmation has sent to {phoneNums} successfully");
        //}

        ////ADD
        //[HttpPost("customer-infor/phone-number/add")]
        //[Authorize(Roles = "CUSTOMER")]
        //public async Task<IActionResult> AddPhoneNumber(string phoneNumber, string otp)
        //{
        //    var email = User.FindFirstValue(ClaimTypes.Email);
        //    if (email == null) return NotFound("Loggined user not found ");
        //    var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
        //    var result = await _projectHelper.VerifyPhoneNum(phoneNumber, otp);
        //    if (!result) return StatusCode(StatusCodes.Status406NotAcceptable,
        //        new Response("Error", "The OTP is not correct"));
        //    loggedInUser.PhoneNumber = phoneNumber;
        //    loggedInUser.PhoneNumberConfirmed = true;
        //    await _dbContext.AddAsync(loggedInUser);
        //    await _dbContext.SaveChangesAsync();
        //    return Accepted("Add phone number successfully");
        //}

        ////UPDATE
        //[HttpPut("customer-infor/phone-number/update")]
        //[Authorize(Roles = "CUSTOMER")]
        //public async Task<IActionResult> ChangeCustomerPhoneNum(string phoneNumber, string otp)
        //{
        //    var email = User.FindFirstValue(ClaimTypes.Email);
        //    if (email == null) return NotFound("Loggined user not found ");
        //    var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
        //    var result = await _projectHelper.VerifyPhoneNum(phoneNumber, otp);
        //    if (!result)
        //    {
        //        return StatusCode(StatusCodes.Status406NotAcceptable,
        //            new Response("Error", "The OTP is incorrect"));
        //    }
        //    loggedInUser.PhoneNumber = phoneNumber;
        //    loggedInUser.PhoneNumberConfirmed = true;
        //    _dbContext.Update(loggedInUser);
        //    await _dbContext.SaveChangesAsync();
        //    return Accepted("Change phone number successfully");
        //}

        //                                                              //ADDRESS
        ////VIEW
        //[HttpGet("customer-infor/address")]
        //[Authorize(Roles = "CUSTOMER")]
        //public async Task<IActionResult> GetCustomerAddress()
        //{
        //    var email = User.FindFirstValue(ClaimTypes.Email);
        //    if (email == null) return NotFound("Loggined user not found ");
        //    var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
        //    var addressList = await _dbContext.UserAddresses.Where(a => a.UserId.Equals(loggedInUser.Id)).Select(a => new
        //    {
        //        Id = a.Address.AddressId,
        //        Street = a.Address.Street,
        //        Ward = a.Address.Ward,
        //        District = a.Address.District,
        //        Provine = a.Address.Provine,
        //        AddressType = a.AddressType
        //    }
        //    ).ToListAsync();
        //    if (addressList.Count == 0) return NotFound("User has no address added");
        //    return Ok(addressList);
        //}

        ////CREATE
        //[HttpPost("customer-infor/address/add")]
        //[Authorize(Roles = "CUSTOMER")]
        //public async Task<IActionResult>AddCustomerAddress([FromForm] AddCustomerAddressViewModel userInput)
        //{
        //    if (userInput.Type.Equals("DEFAULT"))
        //    {
        //        var defaultAddress = await _dbContext.UserAddresses.FirstOrDefaultAsync(ua => ua.AddressType.Equals("DEFAULT"));
        //        if (defaultAddress != null) return StatusCode(StatusCodes.Status406NotAcceptable,
        //            new Response("Error", "Cannot set more than one default address"));
        //    }
        //    var email = User.FindFirstValue(ClaimTypes.Email);
        //    if (email == null) return NotFound("Loggined user not found ");
        //    var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
        //    var newAddress = new Models.Address()
        //    {
        //        Street = userInput.Street,
        //        Ward = userInput.Ward,
        //        District = userInput.District,
        //        Provine = userInput.Provine,
        //        AddressOwner = "USER"
        //    };
        //    await _dbContext.AddAsync(newAddress);
        //    await _dbContext.SaveChangesAsync();
        //    var newUserAddress = new UserAddress()
        //    {
        //        AddressId = newAddress.AddressId,
        //        UserId = loggedInUser.Id,
        //        AddressType = userInput.Type.ToUpper()
        //    };
        //    await _dbContext.AddAsync(newUserAddress);
        //    await _dbContext.SaveChangesAsync();
        //    return Ok("Add address successfully");
        //}
        
        ////UPDATE
        //[HttpPut("customer-infor/address/update")]
        //[Authorize(Roles = "CUSTOMER")]
        //public async Task<IActionResult> UpdateCustomerAddress([FromForm] EditCustomerAddressViewModel userInput)
        //{
        //    var email = User.FindFirstValue(ClaimTypes.Email);
        //    if (email == null) return NotFound("Loggined user not found ");
        //    var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
        //    var currentAddress = await _dbContext.Addresses.FindAsync(userInput.AddressId);
        //    if (currentAddress == null) return NotFound("This address is not exist");
        //    var currentUserAddress = await _dbContext.UserAddresses.FirstOrDefaultAsync(ua => ua.AddressId == currentAddress.AddressId);
        //    if (!userInput.Type.IsNullOrEmpty())
        //    {
        //        if (!currentUserAddress.AddressType.Equals("DEFAULT") && userInput.Type.Equals("DEFAULT")) return BadRequest("An user cannot have more than one default address");

        //    }
        //    currentAddress.Street = userInput.Street == null ? currentAddress.Street : userInput.Street;
        //    currentAddress.Ward = userInput.Ward == null ? currentAddress.Ward : userInput.Ward;
        //    currentAddress.District = userInput.District == null ? currentAddress.District : userInput.District; 
        //    currentAddress.Provine = userInput.Provine == null ? currentAddress.Provine : userInput.Provine;
        //    currentUserAddress.AddressType = userInput.Type == null ? currentUserAddress.AddressType : userInput.Type.ToUpper();
        //    _dbContext.UpdateRange(currentUserAddress, currentAddress);
        //    await _dbContext.SaveChangesAsync();
        //    return Ok("Update address successfully");
        //}
        ////DELETE
        //[HttpDelete("customer-infor/address/remove/{id}")]
        //[Authorize(Roles = "CUSTOMER")]
        //public async Task<IActionResult> RemoveUserAddress([Required]int id)
        //{
        //    var currentAddress = await _dbContext.Addresses.FindAsync(id);
        //    if (currentAddress == null) return NotFound("This address does not exist");
        //    try
        //    {
        //        _dbContext.RemoveRange(currentAddress.UserAddresses.ToArray());
        //        _dbContext.Remove(currentAddress);
        //        await _dbContext.SaveChangesAsync();
        //        return Ok("Remove the address successcully");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError,
        //            new Response("Error", "The address remove failed"));
        //    }

        //}

        //                                                                                             //BASIC USER INFORMATION
        ////VIEW
        //[HttpGet("customer-infor")]
        //[Authorize(Roles = "CUSTOMER")]
        //public async Task<IActionResult> GetCustomerInfor()
        //{
        //    var email = User.FindFirstValue(ClaimTypes.Email);
        //    if (email == null) return NotFound("Loggined user not found ");
        //    var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
        //    var respose = new
        //    {
        //        FirtName = loggedInUser.FirstName,
        //        LastName = loggedInUser.LastName,
        //        DoB = loggedInUser.DoB.Value,
        //        Gender = loggedInUser.Gender,
        //        Avatar = _firebaseService.GetDownloadUrl(loggedInUser.Avatar),
        //        Spent = loggedInUser.Spent,
        //        Debit = loggedInUser.Debit
        //    };
        //    return Ok(respose);
        //}
        ////UPDATE
        //[HttpPut("customer-infor/update")]
        //[Authorize(Roles = "CUSTOMER")]
        //public async Task<IActionResult> UpdateInfor([FromForm] EditInforViewModel userInput)
        //{       
        //    var email = User.FindFirstValue(ClaimTypes.Email);
        //    if (email == null) return NotFound("Loggined user not found ");
        //    var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
        //    loggedInUser.FirstName = userInput.FirstName == null ? loggedInUser.FirstName : userInput.FirstName;
        //    loggedInUser.LastName = userInput.LastName == null ? loggedInUser.LastName : userInput.LastName; ;
        //    loggedInUser.DoB = userInput.DoB == null ? loggedInUser.DoB : userInput.DoB; ;
        //    loggedInUser.Gender = userInput.Gender == null ? loggedInUser.Gender : userInput.Gender;
        //    loggedInUser.LatestUpdate = DateTime.Now;
            
        //    if (userInput.Image != null)
        //    {
        //        bool isRemoved = true;
        //        if (loggedInUser.Avatar != null) isRemoved = _firebaseService.RemoveFile(loggedInUser.Avatar);               
        //        if (!isRemoved) return StatusCode(StatusCodes.Status500InternalServerError,
        //            new Response("Error", "An error occurs when  uploading file"));
        //        loggedInUser.Avatar = _firebaseService.UploadFile(userInput.Image);
        //    }
        //    try 
        //    {
        //        _dbContext.Update(loggedInUser);
        //        await _dbContext.SaveChangesAsync();
        //        return Ok("Update user information successfully");
        //    }
        //    catch
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError,
        //           new Response("Error", "An error occurs when updating user information"));
        //    }
                      
        //}

        ////2FA
        //[HttpGet("customer-infor/2fa-status")]
        //[Authorize(Roles = "CUSTOMER")]
        //public async Task<IActionResult> StatusTwoFactor()
        //{
        //    var email = User.FindFirstValue(ClaimTypes.Email);
        //    if (email == null) return NotFound("Loggined user not found ");
        //    var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
        //    return Ok(loggedInUser.TwoFactorEnabled);
        //}

        ////TURN ON/OFF 2FA
        //[HttpPut("customer-infor/toggle-2fa")]
        //[Authorize(Roles = "CUSTOMER")]
        //public async Task<IActionResult> ToggleTwoFactor()
        //{
        //    var email = User.FindFirstValue(ClaimTypes.Email);
        //    if (email == null) return NotFound("Loggined user not found ");
        //    var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
        //    loggedInUser.TwoFactorEnabled = !loggedInUser.TwoFactorEnabled;
        //    _dbContext.Update(loggedInUser);
        //    await _dbContext.SaveChangesAsync();
        //    return Ok("Successfully");
        //}

        //NOW                                                                             //CHECKOUT
        [HttpGet("checkout-now")]
        [Authorize(Roles = "CUSTOMER")] 
        public async Task<IActionResult> CheckoutNow([Required] string furnitureSpecificationId, [Required] int Quantity)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found ");
            var loggedInUser = await _userManager.FindByEmailAsync(email);
            string  checkCustomerInfor = _projectHelper.CheckInforVerify(loggedInUser);
            if (!checkCustomerInfor.IsNullOrEmpty()) return StatusCode(StatusCodes.Status405MethodNotAllowed,
                new Response("Error", checkCustomerInfor));
            var orderFurniture = await _dbContext.FurnitureSpecifications.FindAsync(furnitureSpecificationId);
            if (orderFurniture == null) return NotFound($"Furnitur specfication with id = {furnitureSpecificationId} is not exist");
            int available = orderFurniture.FurnitureRepositories == null ? 0 : orderFurniture.FurnitureRepositories.Sum(fr => fr.Available);
            if (available == 0) return StatusCode(StatusCodes.Status403Forbidden,
                new Response("Error", "The furniture specification is out of stock"));
            var paymentMethods = await _dbContext.Payments.Select(p => new
            {
                PaymentId = p.PaymentId,
                PaymentMethod = p.PaymentMethod
            }).ToListAsync();
            var deliveryAddress = loggedInUser.UserAddresses.FirstOrDefault(ua => ua.AddressType.Equals("DEFAULT"));
            if (deliveryAddress == null) return NotFound(new Response("Error", "User must add address before check out"));
            var response = new
            {
                DeliveryAddressId = deliveryAddress.AddressId,
                DeliveryAddress = deliveryAddress.Address.ToString(),
                FurnitureId = orderFurniture.FurnitureId,
                FurnitureName = orderFurniture.Furniture.FurnitureName,
                FurnitureSpecificationId = furnitureSpecificationId,
                FurnitureSpecificationName = orderFurniture.FurnitureSpecificationName,
                FurnitureSpecificationImage = orderFurniture.Attachments.IsNullOrEmpty() ? "" : _firebaseService.GetDownloadUrl(orderFurniture.Attachments.First().Path),
                Quantity = Quantity,
                Payments = paymentMethods,
                TotalCost = Math.Round(orderFurniture.Price * Quantity, 2)
            };
            return Ok(response);
        }
        //VIA CART
        [HttpGet("checkout-via-cart")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> CheckoutViaCart([FromQuery] List<int> cartIdList)
        {
            if (cartIdList.IsNullOrEmpty()) return BadRequest("cartId is required");
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            //var checkCustomerInfor = CheckCustomerInfor(loggedInUser);
            //if (checkCustomerInfor != null) return StatusCode(StatusCodes.Status405MethodNotAllowed,
            //    new Response() { Status = "Error", Message = checkCustomerInfor });
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
            var paymentMethods = await _dbContext.Payments.Select(p => new
            {
                PaymentId = p.PaymentId,
                PaymentMethod = p.PaymentMethod
            }).ToListAsync();
            var deliveryAddress = loggedInUser.UserAddresses.FirstOrDefault(ua => ua.AddressType.Equals("DEFAULT"));
            var response = new
            {
                DeliveryAddressId = deliveryAddress.AddressId,
                DeliveryAddress = deliveryAddress.Address.ToString(),
                items = selectedItems,
                TotalCost = selectedItems.Sum(i => i.Cost),
                Payments = paymentMethods

            };
            return Ok(response);
        }
        //CUSTOMIZE FURNITURE
        [HttpGet("checkout-customize-furniture")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> CheckoutCustomizeFurniture([FromQuery] List<string> customizeFurnitureIdList)
        {
            if (customizeFurnitureIdList.IsNullOrEmpty()) return BadRequest("cartId is required");
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found ");
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            //var checkCustomerInfor = CheckCustomerInfor(loggedInUser);
            //if (checkCustomerInfor != null) return StatusCode(StatusCodes.Status405MethodNotAllowed,
            //    new Response() { Status = "Error", Message = checkCustomerInfor });
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
                    Quantity = item.Quantity,
                    Cost = item.Result.ExpectedPrice.Value
                };
                selectedItems.Add(data);
            };
            var paymentMethods = await _dbContext.Payments.Select(p => new
            {
                PaymentId = p.PaymentId,
                PaymentMethod = p.PaymentMethod
            }).ToListAsync();
            var deliveryAddress = loggedInUser.UserAddresses.FirstOrDefault(ua => ua.AddressType.Equals("DEFAULT"));
            var response = new
            {
                DeliveryAddressId = deliveryAddress.AddressId,
                DeliveryAddress = deliveryAddress.Address.ToString(),
                items = selectedItems,
                TotalCost = selectedItems.Sum(i => i.Cost),
                Payments = paymentMethods

            };
            return Ok(response);
        }
                                                                                       
                                                                                            //ORDER
        [HttpPost("order")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> Order([FromBody] OrderViewModel model)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            //var checkCustomerInfor = CheckCustomerInfor(loggedInUser);
            //if (checkCustomerInfor != null) return StatusCode(StatusCodes.Status405MethodNotAllowed,
            //    new Response() { Status = "Error", Message = checkCustomerInfor });
            bool isCustomizeFurnitureOrder = model.Items.FirstOrDefault().ItemId.StartsWith("CF") ? true : false;
            if (isCustomizeFurnitureOrder && model.PaymentId == 1) return BadRequest("Cash payment is not available for customize furniture order");
            if (loggedInUser.Point < model.UsedPoint) return BadRequest("The total point is not enough");
            var deliverAddress = await _dbContext.UserAddresses.FirstOrDefaultAsync(a => a.AddressId == model.AddressId && a.UserId.Equals(loggedInUser.Id));
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
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
              new Response("Error", "An error occurs when adding new order"));
            }
            
            foreach (var item in model.Items)
            {
                
                if (!isCustomizeFurnitureOrder)
                {
                   
                    var furnitureExist  = await _dbContext.FurnitureSpecifications.FindAsync(item.ItemId);
                    if (furnitureExist == null) return BadRequest($"The furniture with id = {item.ItemId} was not found");
                    if (furnitureExist.FurnitureRepositories.IsNullOrEmpty() || furnitureExist.FurnitureRepositories.Sum(fr => fr.Available) == 0)
                        return BadRequest($"Cannot order furniture with id = {furnitureExist.FurnitureId} because it is out of stock"); 
                    var orderItem = new FurnitureOrderDetail()
                    {

                        OrderId = newOrder.OrderId,
                        FurnitureSpecificationId = item.ItemId,
                        Quantity = item.Quantity,
                        Cost = Math.Round(furnitureExist.Price * item.Quantity, 2)
                    };
                    await _dbContext.AddAsync(orderItem);
                }
                else
                {
                    CustomizeFurniture customizeFurniture = await _dbContext.CustomizeFurnitures.FindAsync(item.ItemId);
                    var orderItem = new CustomizeFurnitureOrderDetail()
                    {
                        OrderId = newOrder.OrderId,
                        CustomizeFunitureId = item.ItemId,
                        Quantity = customizeFurniture.Quantity,
                        Cost = customizeFurniture.Result.ExpectedPrice.Value
                    };
                    await _dbContext.AddAsync(orderItem);
                }              
            }
            await _dbContext.SaveChangesAsync();
            newOrder.TotalCost = isCustomizeFurnitureOrder ? newOrder.CustomizeFurnitureOrderDetails.Sum(i => i.Cost) :
                                                               newOrder.FurnitureOrderDetails.Sum(i => i.Cost);
            _dbContext.Update(newOrder);
            await _dbContext.SaveChangesAsync();
            if (newOrder.PaymentId != 1 || isCustomizeFurnitureOrder) return Ok(_projectHelper.UrlPayment(newOrder.PaymentId, newOrder.OrderId));
            return Ok("Order successfully");
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
                        if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                        {
                            //Thanh toan thanh cong
                            var order = await _dbContext.Orders.FindAsync(orderId);
                            if (!order.CustomizeFurnitureOrderDetails.IsNullOrEmpty()) order.Customer.Debit = order.TotalCost / 2;
                            order.IsPaid = true;
                            order.Status = "Preparing";
                            _dbContext.UpdateRange(order, order.Customer);
                            await _dbContext.SaveChangesAsync();
                            return Ok($"Payment success OrderId={orderId}, VNPAY TranId={vnpayTranId}");
                            
                        }
                        else
                        {
                            //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                            return StatusCode(StatusCodes.Status406NotAcceptable, 
                                new Response("error", $"error payment OrderId={orderId}"));
                            
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
            if (order.IsPaid && order.PaymentId != 1)
            {
                var result = _projectHelper.Refund(order);
                if (!result) return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", "An error occur during refund process"));
            }

            //change status
            order.Status = "Canceled";
            _dbContext.Update(order);
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
            if (orderList.Count == 0) return NotFound($"There is no order in status: {status}");
            var response = new List<object>();
            foreach (var order in orderList)
            {
                if (order.TotalCost > 0)
                {
                    var furnitures = order.FurnitureOrderDetails.Select(o => new
                    {
                        FurnitureId = o.FurnitureSpecification.Furniture.FurnitureId,
                        FurnitureName = o.FurnitureSpecification.Furniture.FurnitureName,
                        FurnitureSpecificationId = o.FurnitureSpecification.FurnitureSpecificationId,
                        FurnitureSpecificationname = o.FurnitureSpecification.FurnitureSpecificationName,
                        Quantity = o.Quantity,
                        Cost = o.Cost
                    });
                    var data = new
                    {
                        OrderId = order.OrderId,
                        DeliveryAddress = order.DeliveryAddress,
                        Furniture = furnitures,
                        PaymentMethod = order.Payment.PaymentMethod,
                        TotalCost = order.TotalCost,
                        Status = order.Status

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
            var responses = loggedInUser.Feedbacks.Select(fb => new
            {
                FeedbackId = fb.FeedbackId,
                OrderId = fb.OrderId,
                FurnitureId = fb.FurnitureSpecification.Furniture.FurnitureId,
                FurnitureName = fb.FurnitureSpecification.Furniture.FurnitureName,
                FeedbackImages = fb.Attachements.Count > 0 ? fb.Attachements.Select(a => new
                {
                    url = _firebaseService.GetDownloadUrl(a.Path),
                    type = a.Type
                }) : null,
                Content = fb.Content,
                VoteStar = fb.VoteStar
            });          
            return Ok(responses);
        }

        [HttpPost("create-feedback")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> CreationFeedback([FromForm] FeedbackViewModel model)
        {
            //validation
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound(new Response("Error", "Logged in user not found"));
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));

            var order = await _dbContext.Orders.FindAsync(model.OrderId);
            if (order == null) return NotFound("The order does not exist");
            if (!order.Status.Equals("Delivered")) return StatusCode(StatusCodes.Status406NotAcceptable,
                new Response("Error", "The order must be in \"Delivered\" status to give feedback"));

            FurnitureSpecification specification = order.FurnitureOrderDetails.FirstOrDefault(fod => fod.FurnitureSpecificationId.Equals(model.FurnitureSpecificationId)).FurnitureSpecification;
            if (specification == null) return NotFound($"The order does not contains furniture specification with id = {model.FurnitureSpecificationId}");
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
            PointHistory newPointHistory = new PointHistory()
            {
                CustomerId = loggedInUser.Id,
                Description = $"Give feedback {specification.Furniture.FurnitureName} successfully +200 point",
                History = DateTime.Now
            };

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
            var averageStar = Math.Round(sumStar / (feedbackList.Count+1), 1);
            furniture.VoteStar = averageStar;
            try
            {
                await _dbContext.AddAsync(newFeedback);
                await _dbContext.AddAsync(newPointHistory);
                _dbContext.Update(furniture);
                _dbContext.Update(loggedInUser);
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
                        Path = _firebaseService.UploadFile(file),
                        Type = _firebaseService.ImageOrVideo(file)
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

                                                                            //CUSTOMIZE FURNITURE

        //GET
        [HttpGet("customize-furnitures")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetCustomizeFurniture(string status)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            List<CustomizeFurniture> customizeFurnitures = new List<CustomizeFurniture>();
            if (status.Equals("All")) customizeFurnitures = loggedInUser.CustomizeFurnitures.ToList();
            else customizeFurnitures = loggedInUser.CustomizeFurnitures.Where(cf => cf.Result.Status.Equals(status)).ToList();
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
                    Images = cf.Attachments.Where(a => a.Type.Equals("Images")).Select(a => new
                    {
                        AttachmentName = a.AttachmentName,
                        Path = _firebaseService.GetDownloadUrl(a.Path)
                    }),
                    Videos = cf.Attachments.Where(a => a.Type.Equals("Videos")).Select(a => new
                    {
                        AttachmentName = a.AttachmentName,
                        Path = _firebaseService.GetDownloadUrl(a.Path)
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
            if (email == null) return NotFound("Loggined user not found ");
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            //var checkCustomerInfor = CheckCustomerInfor(loggedInUser);
            //if (checkCustomerInfor != null) return StatusCode(StatusCodes.Status405MethodNotAllowed,
            //    new Response() { Status = "Error", Message = checkCustomerInfor });

            // tao customize furniture de shop owner duyet
            var newCustomizeFurniture = new CustomizeFurniture()
            {
                CustomizeFurnitureId = "CF-"+Guid.NewGuid().ToString().ToUpper(),
                CustomerId = loggedInUser.Id,
                CustomizeFurnitureName = userInput.CustomizeFurnitureName,
                CategoryId = userInput.CategoryId,
                ColorId = userInput.ColorId,
                Height = userInput.Height,
                Width = userInput.Width,
                Length = userInput.Length,
                WoodId = userInput.WoodId,
                Quantity = userInput.Quantity,
                Description = userInput.Description,
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
                        Path = _firebaseService.UploadFile(file),
                        Type = _firebaseService.ImageOrVideo(file)
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
            if (email == null) return NotFound("Loggined user not found ");
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            CustomizeFurniture customizeFurniture = loggedInUser.CustomizeFurnitures.FirstOrDefault(cf => cf.CustomizeFurnitureId.Equals(userInput.CustomizeFurnitureId));
            if (customizeFurniture == null) return NotFound("Customize furniture not found");
            if (!customizeFurniture.Result.Status.Equals("Pending")) return StatusCode(StatusCodes.Status406NotAcceptable,
                new Response("Error", "The customize furniture can only be changed while in the pending status"));

            customizeFurniture.CustomizeFurnitureName = userInput.CustomizeFurnitureName.IsNullOrEmpty()? customizeFurniture.CustomizeFurnitureName : userInput.CustomizeFurnitureName;     
            customizeFurniture.ColorId = !userInput.ColorId.HasValue || userInput.ColorId.Value == 0  ? customizeFurniture.ColorId : userInput.ColorId.Value;
            customizeFurniture.Height = !userInput.Height.HasValue || userInput.Height.Value == 0  ? customizeFurniture.Height : userInput.Height.Value;
            customizeFurniture.Width = !userInput.Width.HasValue || userInput.Width.Value == 0  ? customizeFurniture.Width : userInput.Width.Value;
            customizeFurniture.Length = !userInput.Length.HasValue || userInput.Length.Value == 0  ? customizeFurniture.Length : userInput.Length.Value;
            customizeFurniture.WoodId = !userInput.WoodId.HasValue || userInput.WoodId.Value == 0  ? customizeFurniture.WoodId : userInput.WoodId.Value;
            customizeFurniture.Quantity = !userInput.Quantity.HasValue || userInput.Quantity.Value == 0  ? customizeFurniture.Quantity : userInput.Quantity.Value;
            customizeFurniture.DesiredCompletionDate = !userInput.DesiredCompletionDate.HasValue? customizeFurniture.DesiredCompletionDate : userInput.DesiredCompletionDate.Value;
            _dbContext.Update(customizeFurniture);
            await _dbContext.SaveChangesAsync();
            if (!userInput.Attachments.IsNullOrEmpty())
            {   
                foreach (var attachment in customizeFurniture.Attachments)
                {
                    bool removeResult = _firebaseService.RemoveFile(attachment.Path);
                    if (removeResult) _dbContext.Remove(attachment);
                };
                await _dbContext.SaveChangesAsync();
                foreach (var file in userInput.Attachments)
                {
                   
                    var newAttachment = new CustomizeFurnitureAttachment()
                    {
                        CustomizeFurnitureId  = customizeFurniture.CustomizeFurnitureId,
                        AttachmentName = file.FileName,
                        Path = _firebaseService.UploadFile(file),
                        Type = _firebaseService.ImageOrVideo(file)
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
            if (email == null) return NotFound("Loggined user not found ");
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
            if (email == null) return NotFound("Loggined user not found ");
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
                EstimatedTime = w.EstimatedTime.HasValue ? String.Format("{0:yyyy/MM/dd}",w.EstimatedTime) :  "Processing",
                Attacments = new
                {
                    Images = w.Attachments.Where(a => a.Type.Equals("Images")).Select(a => new
                    {
                        FileName = a.AttachmentName,
                        Path = _firebaseService.GetDownloadUrl(a.Path)
                    }),
                    Videos = w.Attachments.Where(a => a.Type.Equals("Videos")).Select(a => new
                    {
                        FileName = a.AttachmentName,
                        Path = _firebaseService.GetDownloadUrl(a.Path)
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
            if (email == null) return NotFound("Loggined user not found ");
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            var orderExist = loggedInUser.Orders.FirstOrDefault(w => w.OrderId == userInput.OrderId);
            if (orderExist == null) return NotFound($"The order witd id = {userInput.OrderId} does not exist in the ordered list of customer");
            if (!orderExist.Status.Equals("Deliveried")) return BadRequest($"Not allow to create warranty when order is in {orderExist.Status} status");
            
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
            } catch
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
                        Path = _firebaseService.UploadFile(file),
                        Type = _firebaseService.ImageOrVideo(file)
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
            if (email == null) return NotFound("Loggined user not found ");
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            var editWarranty = loggedInUser.Warranties.FirstOrDefault(w => w.WarrantyId == userInput.WarrantyId);
            if (editWarranty == null) return NotFound($"warranty claim with id = {userInput.WarrantyId} was not found in in the customer's warranty claim list");
            
            editWarranty.WarrantyReasons = userInput.WarrantyReasons;

            _dbContext.Update(editWarranty);
            await _dbContext.SaveChangesAsync();
            if (!userInput.UploadFiles.IsNullOrEmpty())
            {
                
                foreach (var attachment in editWarranty.Attachments)
                {                    
                    if (_firebaseService.RemoveFile(attachment.Path))
                    _dbContext.Remove(attachment);
                };
                foreach (var file in userInput.UploadFiles)
                {
                    var newAttachment = new WarrantyAttachment()
                    {
                        WarrantyId = editWarranty.WarrantyId,
                        AttachmentName = file.FileName,
                        Path = _firebaseService.UploadFile(file),
                        Type = _firebaseService.ImageOrVideo(file)
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
        public async Task<IActionResult> RemoveWarranty([Required]int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            Warranty removeWarranty = loggedInUser.Warranties.FirstOrDefault(w => w.WarrantyId == id);
            if (removeWarranty == null) return NotFound($"warranty claim with id = {id} was not found in in the customer's warranty claim list");

            try
            {
                _dbContext.RemoveRange(removeWarranty.Attachments);
                await _dbContext.SaveChangesAsync();
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


        ////SUPPORTED METHODs
        //[ApiExplorerSettings(IgnoreApi = true)]
        //public bool Refund(Order order)
        //{
        //    var transition = GetTransition(order);
        //    if (transition == null) return false;
        //    var vnp_Api = _config["VNPAY:Api"];
        //    var vnp_HashSecret = _config["VNPAY:HashSecret"]; //Secret KEy
        //    var vnp_TmnCode = _config["VNPAY:TmnCode"]; // Terminal Id
            
        //    var vnp_RequestId = DateTime.Now.Ticks.ToString(); //Mã hệ thống merchant tự sinh ứng với mỗi yêu cầu hoàn tiền giao dịch. Mã này là duy nhất dùng để phân biệt các yêu cầu truy vấn giao dịch. Không được trùng lặp trong ngày.
        //    var vnp_Version = VnPayService.VERSION; //2.1.0
        //    var vnp_Command = "refund";
        //    var vnp_TransactionType = "02";
        //    var vnp_Amount = transition.Amount;
        //    var vnp_TxnRef = transition.TxnRef; // Mã giao dịch thanh toán tham chiếu
        //    var vnp_OrderInfo = "Hoan tien giao dich:" + order.OrderId;
        //    var vnp_TransactionNo = transition.TransactionNo;
        //    var vnp_TransactionDate = transition.PayDate;
        //    var vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");
        //    var vnp_CreateBy = order.Customer.UserName;
        //    string ipAddress = Response.HttpContext.Connection.RemoteIpAddress.ToString();
        //    if (ipAddress.Equals("::1")) ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
        //    var vnp_IpAddr = ipAddress;

        //    var signData = vnp_RequestId + "|" + vnp_Version + "|" + vnp_Command + "|" + vnp_TmnCode + "|" + vnp_TransactionType + "|" + vnp_TxnRef + "|" + vnp_Amount + "|" + vnp_TransactionNo + "|" + vnp_TransactionDate + "|" + vnp_CreateBy + "|" + vnp_CreateDate + "|" + vnp_IpAddr + "|" + vnp_OrderInfo;
        //    var vnp_SecureHash = Utils.HmacSHA512(vnp_HashSecret, signData);

        //    var rfData = new
        //    {
        //        vnp_RequestId = vnp_RequestId,
        //        vnp_Version = vnp_Version,
        //        vnp_Command = vnp_Command,
        //        vnp_TmnCode = vnp_TmnCode,
        //        vnp_TransactionType = vnp_TransactionType,
        //        vnp_TxnRef = vnp_TxnRef,
        //        vnp_Amount = vnp_Amount,
        //        vnp_OrderInfo = vnp_OrderInfo,
        //        vnp_TransactionNo = vnp_TransactionNo,
        //        vnp_TransactionDate = vnp_TransactionDate,
        //        vnp_CreateBy = vnp_CreateBy,
        //        vnp_CreateDate = vnp_CreateDate,
        //        vnp_IpAddr = vnp_IpAddr,
        //        vnp_SecureHash = vnp_SecureHash

        //    };
        //    var jsonData = JsonConvert.SerializeObject(rfData);
        //    var httpWebRequest = (HttpWebRequest)WebRequest.Create(vnp_Api);
        //    httpWebRequest.ContentType = "application/json";
        //    httpWebRequest.Method = "POST";

        //    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        //    {
        //        streamWriter.Write(jsonData);
        //    }
        //    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        //    var strData = "";
        //    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        //    {
        //        strData = streamReader.ReadToEnd();
        //    }
        //    if (!strData.Contains("\"vnp_ResponseCode\":\"00\"")) return false;
        //    return true;
        //}

        //[ApiExplorerSettings(IgnoreApi = true)]
        //public VnPayTransition GetTransition(Order order)
        //{
        //    var vnp_Api = _config["VNPAY:Api"];
        //    var vnp_HashSecret = _config["VNPAY:HashSecret"];
        //    var vnp_TmnCode = _config["VNPAY:TmnCode"]; 

        //    var vnp_RequestId = DateTime.Now.Ticks.ToString(); //Mã hệ thống merchant tự sinh ứng với mỗi yêu cầu truy vấn giao dịch. Mã này là duy nhất dùng để phân biệt các yêu cầu truy vấn giao dịch. Không được trùng lặp trong ngày.
        //    var vnp_Version = VnPayService.VERSION; //2.1.0
        //    var vnp_Command = "querydr";
        //    var vnp_TxnRef = order.OrderId.ToString(); // Mã giao dịch thanh toán tham chiếu
        //    var vnp_OrderInfo = "Payment orders:" + order.OrderId;
        //    var vnp_TransactionDate = order.OrderDate.ToString("yyyyMMdd");
        //    var vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");

        //    string ipAddress = Response.HttpContext.Connection.RemoteIpAddress.ToString();
        //    if (ipAddress.Equals("::1")) ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
        //    var vnp_IpAddr = ipAddress;

        //    var signData = vnp_RequestId + "|" + vnp_Version + "|" + vnp_Command + "|" + vnp_TmnCode + "|" + vnp_TxnRef + "|" + vnp_TransactionDate + "|" + vnp_CreateDate + "|" + vnp_IpAddr + "|" + vnp_OrderInfo;
        //    var vnp_SecureHash = Utils.HmacSHA512(vnp_HashSecret, signData);

        //    var qdrData = new
        //    {
        //        vnp_RequestId = vnp_RequestId,
        //        vnp_Version = vnp_Version,
        //        vnp_Command = vnp_Command,
        //        vnp_TmnCode = vnp_TmnCode,
        //        vnp_TxnRef = vnp_TxnRef,
        //        vnp_OrderInfo = vnp_OrderInfo,
        //        vnp_TransactionDate = vnp_TransactionDate,
        //        vnp_CreateDate = vnp_CreateDate,
        //        vnp_IpAddr = vnp_IpAddr,
        //        vnp_SecureHash = vnp_SecureHash

        //    };
        //    var jsonData = JsonConvert.SerializeObject(qdrData);

        //    var httpWebRequest = (HttpWebRequest)WebRequest.Create(vnp_Api);
        //    httpWebRequest.ContentType = "application/json";
        //    httpWebRequest.Method = "POST";

        //    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        //    {
        //        streamWriter.Write(jsonData);
        //    }
        //    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        //    var strData = "";
        //    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        //    {
        //        strData = streamReader.ReadToEnd();
        //    }
        //    if (!strData.Contains("\"vnp_ResponseCode\":\"00\"")) return null;
        //    JObject data = JObject.Parse(strData);
        //    var vnPayTransition = new VnPayTransition()
        //    {
        //            TxnRef = (string) data["vnp_TxnRef"],
        //            Amount = (string)data["vnp_Amount"],
        //            OrderInfo = (string)data["vnp_OrderInfo"],
        //            BankCode = (string)data["vnp_BankCode"],
        //            PayDate = (string)data["vnp_PayDate"],
        //            TransactionNo = (string)data["vnp_TransactionNo"],
        //            TransactionType = (string)data["vnp_TransactionType"],
        //            TransactionStatus = (string)data["vnp_TransactionStatus"],
        //    };
        //    return vnPayTransition;
        //}

        //[ApiExplorerSettings(IgnoreApi = true)]
        //public string UrlPayment(int typePayment, int orderId)
        //{

        //    var order = _dbContext.Orders.FirstOrDefault(o => o.OrderId == orderId);

        //    //Get Config Info
        //    string vnp_Returnurl = _config["VNPAY:ReturnUrl"]; //URL nhan ket qua tra ve 
        //    string vnp_Url = _config["VNPAY:Url"]; //URL thanh toan cua VNPAY 
        //    string vnp_TmnCode = _config["VNPAY:TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
        //    string vnp_HashSecret = _config["VNPAY:HashSecret"]; //Secret Key
        //    //Build URL for VNPAY
        //    VnPayService vnpay = new VnPayService();
        //    vnpay.AddRequestData("vnp_Version", VnPayService.VERSION);
        //    vnpay.AddRequestData("vnp_Command", "pay");
        //    vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
        //    if (!order.CustomizeFurnitureOrderDetails.IsNullOrEmpty()) 
        //    {
        //        vnpay.AddRequestData("vnp_Amount", (order.TotalCost * 100000/2).ToString()); // thanh toán trước 50% nếu là đơn customize
        //    } else
        //    {
        //        vnpay.AddRequestData("vnp_Amount", (order.TotalCost * 100000).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
        //    }
        //    var payment = _dbContext.Payments.Find(typePayment);
        //    vnpay.AddRequestData("vnp_BankCode", payment.PaymentMethod);
        //    vnpay.AddRequestData("vnp_CreateDate", order.OrderDate.ToString("yyyyMMddHHmmss"));
        //    vnpay.AddRequestData("vnp_CurrCode", "VND");
        //    string ipAddress = Response.HttpContext.Connection.RemoteIpAddress.ToString();
        //    if (ipAddress.Equals("::1")) ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
        //    vnpay.AddRequestData("vnp_IpAddr", ipAddress);
        //    vnpay.AddRequestData("vnp_Locale", "en");
        //    vnpay.AddRequestData("vnp_OrderInfo", "Payment orders:" + order.OrderId);
        //    vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
        //    vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
        //    vnpay.AddRequestData("vnp_TxnRef", order.OrderId.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày
        //    string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
        //    return paymentUrl;
        //}

        //[ApiExplorerSettings(IgnoreApi = true)]
        //public async Task<bool> VerifyPhoneNum(string phoneNums, string totpCode)
        //{
        //    byte[] secretKey = Encoding.ASCII.GetBytes(phoneNums);
        //    var otp = new Totp(secretKey, step: 300, totpSize: 6);
        //    var window = new VerificationWindow(previous: 1, future: 1);
        //    long timeStepMatched;
        //    var result = otp.VerifyTotp(DateTime.Now, totpCode, out timeStepMatched, window);
        //    if (!result)
        //    {
        //        return false;
        //    }
        //    return true;
        //}
        
        //[ApiExplorerSettings(IgnoreApi = true)]
        //private JwtSecurityToken GenerateJWTToken(User user, IList<string> roles)
        //{
        //    var authClaims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Email, user.Email),
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //    };
        //    foreach (var role in roles)
        //    {
        //        authClaims.Add(new Claim(ClaimTypes.Role, role));
        //    }
        //    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));
        //    var token = new JwtSecurityToken(
        //        issuer: _config["JWT:ValidIssuer"],
        //        audience: _config["JWT:ValidAudience"],
        //        expires: DateTime.Now.AddDays(2),
        //        claims: authClaims,
        //        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        //        );
        //    return token;
        //}

        //[ApiExplorerSettings(IgnoreApi = true)]
        //public async Task<bool> CreateUserInfor(string email)
        //{
        //    var user = await _userManager.FindByEmailAsync(email);
        //    if (user == null) return false;

        //    var userPoint = new Models.Purchase.Point()
        //    {
        //        CustomerId = user.Id,
        //        User = user,
        //        Description = "Create account successfully +5 point",
        //        TotalPoint = 5,
        //        History = DateTime.Now
        //    };
        //    var announcement = new Announcement()
        //    {
        //        UserId = user.Id,
        //        User = user,
        //        Title = "Welcome",
        //        Content = "Welcome to Furniture Shopping Online, wish you have a great shopping experience!",
        //        CreationDate = DateTime.Now
        //    };
        //    var cart = new Cart()
        //    {
        //        CustomerId = user.Id
        //    };
        //    await _dbContext.AddRangeAsync(announcement, userPoint, cart);
        //    await _dbContext.SaveChangesAsync();
        //    return true;
        //}

        //[ApiExplorerSettings(IgnoreApi = true)]
        //public string CheckCustomerInfor(User customer)
        //{
        //    if (customer.UserAddresses.Count == 0 || customer.UserAddresses == null) return "The customer must add address to use this fuction";
        //    if (customer.PhoneNumber == null || !customer.PhoneNumberConfirmed) return "The customer must add phone number and verify it to use this fuction";
        //    return null;
        //}
    }
}
