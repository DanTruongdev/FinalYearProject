using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OnlineShopping.Data;
using OnlineShopping.Libraries.Models;
using OnlineShopping.Libraries.Services;
using OnlineShopping.ViewModels;
using OnlineShopping.ViewModels.Login;
using OnlineShopping.ViewModels.SignUp;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using OnlineShopping.Models.Funiture;
using OtpNet;
using OnlineShopping.Models;
using OnlineShopping.Models.Purchase;
using OnlineShopping.Models.Customer;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR;
using OnlineShopping.ViewModels.User;
using Org.BouncyCastle.Crypto.Operators;
using Microsoft.AspNet.SignalR.Hosting;
using OnlineShopping.ViewModels.Order;
using System.Net;
using System.Web;
using OnlineShopping.ViewModels.VNPAY;
using Newtonsoft.Json.Linq;
using OnlineShopping.ViewModels.Feedback;
using OnlineShopping.Models.Gallery;
using OnlineShopping.ViewModels.Address;
using Castle.Core.Internal;
using OnlineShopping.ViewModels.Furniture;
using Microsoft.AspNetCore.Cors;
using Bogus.DataSets;
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
        public CustomerController(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> userRole,
            SignInManager<User> signInManager, IConfiguration config, IEmailService emailService, ISMSService smsService)
        {
            _dbContext = context;
            _userManager = userManager;
            _roleManager = userRole;
            _signInManager = signInManager;
            _config = config;
            _emailService = emailService;
            _smsService = smsService;
        }

                                                // AUTHORIZATION/ AUTHENTICATION
        [HttpPost("create-account")]
        public async Task<IActionResult> RegisterAccount([FromBody] RegisterUser userInfor) 
        {


            var mailChecker = await _userManager.FindByEmailAsync(userInfor.Email);

            if (mailChecker != null)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable,
                    new Response() { Status = "Error", Message = "The email already used!" });
            }
            var phoneChecker = _userManager.Users.FirstOrDefault(u => u.PhoneNumber.Equals(userInfor.PhoneNumbers));
            if (phoneChecker != null)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable,
                    new Response() { Status = "Error", Message = "The phone numbers already used!" });
            };
            var newUser = new User()
            {
                FirstName = userInfor.FirstName,
                LastName = userInfor.LastName,
                DoB = userInfor.DoB,
                Gender = userInfor.Gender,
                UserName = userInfor.Email,
                PhoneNumber = userInfor.PhoneNumbers,
                Email = userInfor.Email,
                CreationDate = DateTime.Now,
                IsActivated = true,
                TwoFactorEnabled = false                       
            };
            var result = await _userManager.CreateAsync(newUser, userInfor.Password);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status409Conflict,
                    new Response() { Status = "Error", Message = result.ToString() });
            }
            await CreateUserInfor(newUser.Email);
            await _userManager.AddToRoleAsync(newUser, "CUSTOMER");
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var confirmationLink = Url.Action(nameof(ConfirmEmail),
                "Customer", new { token, email = newUser.Email }, Request.Scheme);
            var message = new Message(new string[] { newUser.Email! },
                "Confirmation email link", $"Please click to the following Url to verify your email: \n {confirmationLink!}");
            _emailService.SendEmail(message);
            return StatusCode(StatusCodes.Status201Created,
                    new Response { Status = "Success", Message = $"Account created & a confirmation email sent to {newUser.Email} successFully" });
        }
        
        [HttpGet("confirm-email")]
        public async Task<RedirectResult> ConfirmEmail(string token, string email)
        {
            var userExist = await _userManager.FindByEmailAsync(email);
            if (userExist != null)
            {
                var result = await _userManager.ConfirmEmailAsync(userExist, token);
                if (!result.Succeeded)
                {
                    //return StatusCode(StatusCodes.Status500InternalServerError,
                    // new Response() { Status = "Error", Message = result.ToString() });
                    return RedirectPermanent("https://www.google.com");
                }

                //return StatusCode(StatusCodes.Status200OK,
                        //new Response() { Status = "Success", Message = $"Confim email sucessfully!" });
                return RedirectPermanent("https://www.google.com");
            }
            //return StatusCode(StatusCodes.Status500InternalServerError,
                       // new Response() { Status = "Error", Message = "This user is not exist" });
            return RedirectPermanent("https://www.google.com");
        }
       
        [HttpGet("signin-google")]
        public IActionResult SignInGoogle()
        {
            var redirectUri = Url.Action(nameof(HandleGoogleCallback), "Customer");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUri);
            return new ChallengeResult("Google", properties);
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> HandleGoogleCallback()
        {
            ExternalLoginInfo externalUserInfor = null;
            var externalEmail = "";
            User userWithExternalMail = null;
            try
            {
                externalUserInfor = await _signInManager.GetExternalLoginInfoAsync();
                externalEmail = externalUserInfor.Principal.FindFirstValue(ClaimTypes.Email);
                userWithExternalMail = await _userManager.FindByEmailAsync(externalEmail);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                       new Response() { Status = "Error", Message = "An error occurs during login" });
            }
            try 
            {
                //truong hop da co tai khoan trong db
                if (userWithExternalMail != null)
                {
                    //confirm luon email
                    if (!userWithExternalMail.EmailConfirmed)
                    {
                        var codeactive = await _userManager.GenerateEmailConfirmationTokenAsync(userWithExternalMail);
                        await _userManager.ConfirmEmailAsync(userWithExternalMail, codeactive);
                    }
                    // Thực hiện liên kết info và user

                    var resultAdd = await _userManager.AddLoginAsync(userWithExternalMail, externalUserInfor);
                    if (resultAdd.Succeeded || resultAdd.ToString().Equals("Failed : LoginAlreadyAssociated"))
                    {
                        // Thực hiện login    
                        await _signInManager.SignInAsync(userWithExternalMail, isPersistent: false);
                        var jwtToken = GenerateJWTToken(userWithExternalMail, await _userManager.GetRolesAsync(userWithExternalMail));
                        //return the token
                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                            expiration = jwtToken.ValidTo
                        });
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError,
                           new Response() { Status = "Error", Message = resultAdd.ToString() });
                    }
                }
                //truong hop chua co tai khoan trong db
                var newUser = new User()
                {
                    UserName = $"user@{externalUserInfor.Principal.FindFirstValue(ClaimTypes.NameIdentifier)}",
                    FirstName = externalUserInfor.Principal.FindFirstValue(ClaimTypes.GivenName),
                    LastName = externalUserInfor.Principal.FindFirstValue(ClaimTypes.Surname),
                    DoB = DateTime.Now,
                    Gender = "Other",
                    Email = externalEmail,
                    EmailConfirmed = true,
                    CreationDate = DateTime.Now,
                    IsActivated = true,
                    TwoFactorEnabled = false,
                };
                var result = await _userManager.CreateAsync(newUser);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, "CUSTOMER");
                    result = await _userManager.AddLoginAsync(newUser, externalUserInfor);
                    await _signInManager.SignInAsync(newUser, isPersistent: false, externalUserInfor.LoginProvider);
                    await CreateUserInfor(newUser.Email);
                    var jwtToken = GenerateJWTToken(newUser, await _userManager.GetRolesAsync(newUser));
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                        expiration = jwtToken.ValidTo
                    });
                }

            } 
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                       new Response() { Status = "Error", Message = "Can not add login with external account" });
            }
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUser loginModel)
        {
            var logginedUser = await _userManager.FindByEmailAsync(loginModel.Email);
            var passwordChecker = await _userManager.CheckPasswordAsync(logginedUser, loginModel.Password);
            //checking if the user existed and password was valid
            if (logginedUser == null || !passwordChecker || !logginedUser.IsActivated)
            {
                return Unauthorized();
            }

            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(logginedUser, loginModel.Password, loginModel.RememberMe.Value, true);      
            
            if (logginedUser.TwoFactorEnabled)
            {               
                var token = await _userManager.GenerateTwoFactorTokenAsync(logginedUser, "Email");
                var message = new Message(new string[] { logginedUser.Email! }, "OTP Confirmation", token);
                _emailService.SendEmail(message);
                return StatusCode(StatusCodes.Status200OK,
                 new Response { Status = "Success", Message = $"The system has sent an OTP to your email {logginedUser.Email}" });
            }  
                        
            //generate token with the claims
            var jwtToken = GenerateJWTToken(logginedUser, await _userManager.GetRolesAsync(logginedUser));
            //return the token
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                expiration = jwtToken.ValidTo
            });           
        }

        [HttpPost("login-2FA")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> LoginOTP(string code, string email)
        {          
            var user = await _userManager.FindByEmailAsync(email);         
            var signIn = await _signInManager.TwoFactorSignInAsync("Email", code, false, false);
            if (signIn.Succeeded)
            {
                if (user != null)
                {                                                         
                        var jwtToken = GenerateJWTToken(user, await _userManager.GetRolesAsync(user));
                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                            expiration = jwtToken.ValidTo
                        });
                }
            }
            return Unauthorized();
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([Required] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                //generate a reset password token
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var forgotPasswordLink = Url.Action(nameof(ResetPassword), "Customer", new { token, email = user.Email }, Request.Scheme);
                var message = new Message(new string[] { user.Email }, "Reset password link", forgotPasswordLink);
                _emailService.SendEmail(message);
                return StatusCode(StatusCodes.Status200OK,
                   new Response { Status = "Success", Message = $"Reset password link has sent to  {user.Email} successfully" });
            }
            return StatusCode(StatusCodes.Status400BadRequest,
                   new Response { Status = "Error", Message = "This email is not linked to any account" });
        }

        [HttpGet("reset-password")]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            var model = new ResetPassword { Email = email, Token = token };
            return Ok(new { model });
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
 
        public async Task<IActionResult> SetNewPassword(ResetPassword resetPassword)
        {
           
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user != null)
            {
                //reset password
                var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
                if (!resetPassResult.Succeeded)
                {
                    foreach (var error in resetPassResult.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return Ok(ModelState);
                }
                return StatusCode(StatusCodes.Status200OK,
                   new Response { Status = "Success", Message = "Password has changed successfully!" });
            }
            return StatusCode(StatusCodes.Status400BadRequest,
                   new Response { Status = "Error", Message = "Incorrect email. Please try again!" });
        }

                                                                //FURNITURE
        [HttpGet("furnitures")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllFurniture() //thieu availabe
        {         
            var furnitures = await _dbContext.Furnitures.ToListAsync();
            var response = furnitures.Select(f => new
            {
                FurnitureId = f.FurnitureId,
                FurnitureName = f.FurnitureName,
                VoteStar = f.VoteStar,
                Sold = f.Sold,
                Label = f.Label.LabelName,
                //Available = f.FurnitureSpecifications.IsNullOrEmpty() ? 0 : f.FurnitureSpecifications.Sum(fs => fs.FurnitureRepositories.Sum(fr => fr.Available)),
                Price = f.FurnitureSpecifications.Max(fs => fs.Price) == f.FurnitureSpecifications.Min(fs => fs.Price)
                          ? $"{f.FurnitureSpecifications.Max(fs => fs.Price)}" : $"{f.FurnitureSpecifications.Min(fs => fs.Price)} - {f.FurnitureSpecifications.Max(fs => fs.Price)}",
                Image = f.FurnitureSpecifications.FirstOrDefault().Attachments.IsNullOrEmpty() ? String.Empty :
                        f.FurnitureSpecifications.FirstOrDefault().Attachments.FirstOrDefault().Path
            }).ToList();
           
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
                    FurnitureSpecificationId = fs.FurnitureSpecificationId,
                    FurnitureId = fs.FurnitureId,
                    FurnitureName = fs.Furniture.FurnitureName,
                    Height = fs.Height,
                    Width = fs.Width,
                    Length = fs.Length,
                    Color = fs.Color.ColorName,
                    Wood = fs.Wood.WoodType,
                    Description = fs.Description,
                    Images = fs.Attachments.Where(a => a.Type.Equals("Images")).Select(a => new
                    {
                        AttachmentName = a.AttachmentName,
                        Path = a.Path
                    }),
                    Videos = fs.Attachments.Where(a => a.Type.Equals("Videos")).Select(a => new
                    {
                        AttachmentName = a.AttachmentName,
                        Path = a.Path
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
        public async Task<IActionResult> SearchFurniture(string keyword)
        {
            if (!keyword.IsNullOrEmpty()) keyword = keyword.ToUpper();
            var result = await _dbContext.Furnitures.Where(f => f.FurnitureName.ToUpper().Contains(keyword)).ToListAsync();
            if (result.Count == 0)
            {
                return NotFound();
            }
            var response = result.Select(f => new
            {
                FurnitureId = f.FurnitureId,
                FurnitureName = f.FurnitureName,
                VoteStar = f.VoteStar,
                Sold = f.Sold,
                Label = f.Label.LabelName,
                //Available = f.FurnitureSpecifications.IsNullOrEmpty() ? 0 : f.FurnitureSpecifications.Sum(fs => fs.FurnitureRepositories.Sum(fr => fr.Available)),
                Price = f.FurnitureSpecifications.Max(fs => fs.Price) == f.FurnitureSpecifications.Min(fs => fs.Price)
                          ? $"{f.FurnitureSpecifications.Max(fs => fs.Price)}" : $"{f.FurnitureSpecifications.Min(fs => fs.Price)} - {f.FurnitureSpecifications.Max(fs => fs.Price)}",
                Image = f.FurnitureSpecifications.FirstOrDefault().Attachments.IsNullOrEmpty() ? String.Empty :
                        f.FurnitureSpecifications.FirstOrDefault().Attachments.FirstOrDefault().Path
            }).ToList();
            return Ok(response);
        }
        
        [HttpGet("furnitures/filter")] //thieu image
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
                //Image = gr.FirstOrDefault().Attachment.FirstOrDefault().Path,
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
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            var cartDetail = await _dbContext.CartDetails.Include(cd => cd.FurnitureSpecifition).Include(cd => cd.FurnitureSpecifition.Furniture)
                .Where(cd => cd.CartId == logginedUser.Cart.CartId).ToListAsync();
            var usercCart = cartDetail.Select(cd => new
            {
                FurnitureId = cd.FurnitureSpecifition.Furniture.FurnitureId,
                FurnitureName = cd.FurnitureSpecifition.Furniture.FurnitureName,
                FurnitureSpecificationId = cd.FurnitureSpecificationId,
                FurnitureSpecificationName = cd.FurnitureSpecifition.FurnitureSpecificationName,
                Quantity = cd.Quantity,
                Cost = cd.Quantity * cd.FurnitureSpecifition.Price
            }); 
            return Ok(usercCart);
        }

        [HttpPost("cart/add")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> AddToCart(string furnitureSpecificationId, int quantity)
        {
            if (furnitureSpecificationId == null || quantity == null) return BadRequest();
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            var furnitureSpecification = await _dbContext.FurnitureSpecifications.FindAsync(furnitureSpecificationId);
            if (furnitureSpecification == null) return NotFound("The furniture not found");
           
            //kiem tra san pham da co trong gio hang chua
            var cartDetailExist = await _dbContext.CartDetails.FirstOrDefaultAsync(cd => cd.CartId == logginedUser.Cart.CartId
                                           && cd.FurnitureSpecificationId.Equals(furnitureSpecificationId));
            if (cartDetailExist != null)
            {
                if (quantity == 0)
                {
                    _dbContext.Remove(cartDetailExist);                                     
                    await _dbContext.SaveChangesAsync();
                    return Ok("The furniture has been removed from cart");
                }
                else
                {
                    cartDetailExist.Quantity = quantity;
                    logginedUser.Cart.CartDetails.FirstOrDefault(cd => cd.CartDetailId == cartDetailExist.CartId).Quantity = quantity;
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
                {   CartId =   logginedUser.Cart.CartId,                              
                    FurnitureSpecificationId = furnitureSpecificationId,
                    Quantity = quantity,                   
                };
                await _dbContext.AddAsync(newCartDetail);
                logginedUser.Cart.CartDetails.Add(newCartDetail);
            }
            await _dbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status200OK,
                   new Response() { Status = "Success", Message = "Add the furniture to cart suuccessfully" });
        }

        [HttpDelete("cart/remove/{id}")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> DeleteCartItem(string id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            var cartItems = logginedUser.Cart.CartDetails.ToList();
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
                    new Response() { Status = "Error", Message = "Remove furniture from cart failed" });
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

        [HttpPut("wishlist")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetWishlist()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.Include(u => u.WishList).FirstOrDefaultAsync(u => u.Email.Equals(email));
            var wishlist = logginedUser.WishList.WishListDetails.ToList();
            if (wishlist.IsNullOrEmpty()) return Ok("No furniture in wishlist");
            var response = wishlist.Select(w => new
            {
                FurnitureId = w.Furniture.FurnitureId,
                FurnitureName = w.Furniture.FurnitureName,
                VoteStar = w.Furniture.VoteStar,
                Sold = w.Furniture.Sold,
                Label = w.Furniture.Label.LabelName,
                //Available = f.FurnitureSpecifications.IsNullOrEmpty() ? 0 : f.FurnitureSpecifications.Sum(fs => fs.FurnitureRepositories.Sum(fr => fr.Available)),
                Price = w.Furniture.FurnitureSpecifications.Max(fs => fs.Price) == w.Furniture.FurnitureSpecifications.Min(fs => fs.Price)
                           ? $"{w.Furniture.FurnitureSpecifications.Max(fs => fs.Price)}" : $"{w.Furniture.FurnitureSpecifications.Min(fs => fs.Price)} - {w.Furniture.FurnitureSpecifications.Max(fs => fs.Price)}",
                Image = w.Furniture.FurnitureSpecifications.FirstOrDefault().Attachments.IsNullOrEmpty() ? String.Empty :
                         w.Furniture.FurnitureSpecifications.FirstOrDefault().Attachments.FirstOrDefault().Path
            }).ToList();
            return Ok(response);
        }


        [HttpPut("wishlist/toggle")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> ToggleWishlist(int furnitureId)
        
        {
            if (furnitureId == null) return BadRequest();
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.Include(u => u.WishList).FirstOrDefaultAsync(u => u.Email.Equals(email));
            var furniture = await _dbContext.Furnitures.FindAsync(furnitureId);
            if (furniture == null) return NotFound("The furniture not found");
            var wishListDetailExist = await _dbContext.WishListDetails.FirstOrDefaultAsync(w => w.WishListId == logginedUser.WishList.WishlistId
                                         && w.FurnitureId == furnitureId);
            if (wishListDetailExist != null)
            {
                _dbContext.Remove(wishListDetailExist);
                logginedUser.WishList.WishListDetails.Remove(wishListDetailExist);
                _dbContext.Update(logginedUser);
                await _dbContext.SaveChangesAsync();
                return Ok("The furniture has been removed from wishlist");
            }
            else
            {
                var newWishlistItem = new WishListDetail()
                {
                    WishListId = logginedUser.WishList.WishlistId,
                    WishList = logginedUser.WishList,
                    FurnitureId = furnitureId,
                    Furniture = furniture
                };
                await _dbContext.AddAsync(newWishlistItem);
                logginedUser.WishList.WishListDetails.Add(newWishlistItem);
                _dbContext.Update(logginedUser);
                await _dbContext.SaveChangesAsync();
                return Ok("Add funiture to wishlist succefully");
            }
        }

                                                                             //PHONE NUMBER
        //VIEW
        [HttpGet("customer-infor/phone-number")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetCustomerPhoneNum()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            if (logginedUser.PhoneNumber == null) return NotFound("User has no phonenumber added"); // user login via Google usually lack of phoneNum
            var response = new
            {
                PhoneNumber = logginedUser.PhoneNumber,
                Confirm = logginedUser.PhoneNumberConfirmed
             };
            return Ok(response);  
        }

        [HttpGet("customer-infor/phone-number/get-otp")] 
        public async Task<IActionResult> SendOTPConfirmation(string phoneNums)
        {   var phoneExist = await _dbContext.Users.Where(u => u.PhoneNumber.Equals(phoneNums) && u.PhoneNumberConfirmed == true).ToListAsync();
            if (phoneExist != null) return StatusCode(StatusCodes.Status406NotAcceptable, 
                new Response() { Status = "Error", Message = "This phone number is already linked to another account" });
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            byte[] secretKey = Encoding.ASCII.GetBytes(phoneNums);
            var otp = new Totp(secretKey, step: 300, totpSize: 6);
            var totpCode = otp.ComputeTotp(DateTime.Now);
            var sms = new Sms(new string[] { phoneNums }, $"The OTP to confirm your phone number: {totpCode}");
            _smsService.SendSMS(sms);
            return Ok($"The confirmation has sent to {phoneNums} successfully");
        }

        //ADD
        [HttpPost("customer-infor/phone-number/add")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> AddPhoneNumber(string phoneNumber, string otp)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            var result = await VerifyPhoneNum(phoneNumber, otp);
            if (!result) return StatusCode(StatusCodes.Status406NotAcceptable,
                new Response() { Status = "Error", Message = "The OTP is not correct" });
            logginedUser.PhoneNumber = phoneNumber;
            logginedUser.PhoneNumberConfirmed = true;
            await _dbContext.AddAsync(logginedUser);
            await _dbContext.SaveChangesAsync();
            return Accepted("Add phone number successfully");
        }

        //UPDATE
        [HttpPut("customer-infor/phone-number/update")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> ChangeCustomerPhoneNum(string phoneNumber, string otp)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            var result = await VerifyPhoneNum(phoneNumber, otp);
            if (!result)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable,
                    new Response() { Status = "Error", Message = "The OTP is incorrect" });
            }
            logginedUser.PhoneNumber = phoneNumber;
            logginedUser.PhoneNumberConfirmed = true;
            _dbContext.Update(logginedUser);
            await _dbContext.SaveChangesAsync();
            return Accepted("Change phone number successfully");
        }

                                                                      //ADDRESS
        //VIEW
        [HttpGet("customer-infor/address")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetCustomerAddress()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            var addressList = await _dbContext.UserAddresses.Where(a => a.UserId.Equals(logginedUser.Id)).Select(a => new
            {
                Id = a.Address.AddressId,
                Street = a.Address.Street,
                Ward = a.Address.Ward,
                District = a.Address.District,
                Provine = a.Address.Provine,
                AddressType = a.AddressType
            }
            ).ToListAsync();
            if (addressList.Count == 0) return NotFound("User has no address added");
            return Ok(addressList);
        }

        //CREATE
        [HttpPost("customer-infor/address/add")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult>AddCustomerAddress([FromForm] AddCustomerAddressViewModel userInput)
        {
            if (userInput.Type.Equals("DEFAULT"))
            {
                var defaultAddress = await _dbContext.UserAddresses.FirstOrDefaultAsync(ua => ua.AddressType.Equals("DEFAULT"));
                if (defaultAddress != null) return StatusCode(StatusCodes.Status406NotAcceptable,
                    new Response() { Status = "Error", Message = "Cannot set more than one default address" });
            }
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            var newAddress = new Models.Address()
            {
                Street = userInput.Street,
                Ward = userInput.Ward,
                District = userInput.District,
                Provine = userInput.Provine,
                AddressOwner = "USER"
            };
            await _dbContext.AddAsync(newAddress);
            await _dbContext.SaveChangesAsync();
            var newUserAddress = new UserAddress()
            {
                AddressId = newAddress.AddressId,
                UserId = logginedUser.Id,
                AddressType = userInput.Type.ToUpper()
            };
            await _dbContext.AddAsync(newUserAddress);
            await _dbContext.SaveChangesAsync();
            return Ok("Add address successfully");
        }
        
        //UPDATE
        [HttpPut("customer-infor/address/update")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> UpdateCustomerAddress([FromForm] EditCustomerAddressViewModel userInput)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            var currentAddress = await _dbContext.Addresses.FindAsync(userInput.AddressId);
            if (currentAddress == null) return NotFound("This address is not exist");
            var currentUserAddress = await _dbContext.UserAddresses.FirstOrDefaultAsync(ua => ua.AddressId == currentAddress.AddressId);
            if (!userInput.Type.IsNullOrEmpty())
            {
                if (!currentUserAddress.AddressType.Equals("DEFAULT") && userInput.Type.Equals("DEFAULT")) return BadRequest("An user cannot have more than one default address");

            }
            currentAddress.Street = userInput.Street == null ? currentAddress.Street : userInput.Street;
            currentAddress.Ward = userInput.Ward == null ? currentAddress.Ward : userInput.Ward;
            currentAddress.District = userInput.District == null ? currentAddress.District : userInput.District; 
            currentAddress.Provine = userInput.Provine == null ? currentAddress.Provine : userInput.Provine;
            currentUserAddress.AddressType = userInput.Type == null ? currentUserAddress.AddressType : userInput.Type.ToUpper();
            _dbContext.UpdateRange(currentUserAddress, currentAddress);
            await _dbContext.SaveChangesAsync();
            return Ok("Update address successfully");
        }
        //DELETE
        [HttpDelete("customer-infor/address/remove/{id}")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> RemoveUserAddress([Required]int id)
        {
            var currentAddress = await _dbContext.Addresses.FindAsync(id);
            if (currentAddress == null) return NotFound("This address does not exist");
            try
            {
                _dbContext.RemoveRange(currentAddress.UserAddresses.ToArray());
                _dbContext.Remove(currentAddress);
                await _dbContext.SaveChangesAsync();
                return Ok("Remove the address successcully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response() { Status = "Error", Message = "The address remove failed" });
            }

        }

                                                                                                     //BASIC USER INFORMATION
        //VIEW
        [HttpGet("customer-infor")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetCustomerInfor()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            var respose = new
            {
                FirtName = logginedUser.FirstName,
                LastName = logginedUser.LastName,
                DoB = logginedUser.DoB.Value,
                Gender = logginedUser.Gender,
                Avatar = logginedUser.Avatar,
                Spent = logginedUser.Spent,
                Debit = logginedUser.Debit
            };
            return Ok(respose);
        }
        //UPDATE
        [HttpPut("customer-infor/update")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> UpdateInfor([FromForm] EditCustomerInforViewModel userInput)
        {
            var fileHandle = new FileHandleService();
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            logginedUser.FirstName = userInput.FirstName == null ? logginedUser.FirstName : userInput.FirstName;
            logginedUser.LastName = userInput.LastName == null ? logginedUser.LastName : userInput.LastName; ;
            logginedUser.DoB = userInput.DoB == null ? logginedUser.DoB : userInput.DoB; ;
            logginedUser.Gender = userInput.Gender == null ? logginedUser.Gender : userInput.Gender;
            logginedUser.LatestUpdate = DateTime.Now;
            
            if (userInput.Image != null)
            {
                if (logginedUser.Avatar != null) fileHandle.DeleteFile(logginedUser.Avatar);
                var result = fileHandle.UploadFile("User", userInput.Image);
                if (result.Equals("Error")) return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response() { Status = "Error", Message = "An error occurs during upload file"});
                logginedUser.Avatar = result;
            }
            _dbContext.Update(logginedUser);
            await _dbContext.SaveChangesAsync();
            return Ok("Update user information successfully");
        }

        //2FA
        [HttpGet("customer-infor/2fa-status")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> StatusTwoFactor()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            return Ok(logginedUser.TwoFactorEnabled);
        }
        //TURN ON/OFF 2FA
        [HttpPut("customer-infor/toggle-2fa")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> ToggleTwoFactor()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            logginedUser.TwoFactorEnabled = !logginedUser.TwoFactorEnabled;
            _dbContext.Update(logginedUser);
            await _dbContext.SaveChangesAsync();
            return Ok("Successfully");
        }

        //NOW                                                                             //CHECKOUT
        [HttpGet("checkout-now")]
        [Authorize(Roles = "CUSTOMER")] //thieu image o response
        public async Task<IActionResult> CheckoutNow(string furnitureSpecificationId, int Quantity)
        {
            if (furnitureSpecificationId.IsNullOrEmpty() || Quantity < 1) return BadRequest("FurniturSpecficationID or quantity is required");
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            //var checkCustomerInfor = CheckCustomerInfor(logginedUser);
            //if (checkCustomerInfor != null) return StatusCode(StatusCodes.Status405MethodNotAllowed,
            //    new Response() { Status = "Error", Message = checkCustomerInfor });
            var orderFurniture = await _dbContext.FurnitureSpecifications.FindAsync(furnitureSpecificationId);
            if (orderFurniture == null) return NotFound("FurniturSpecficationID is not exist");
            int available = orderFurniture.FurnitureRepositories.Sum(fr => fr.Available);
            //if (available == 0) return StatusCode(StatusCodes.Status403Forbidden,
            //    new Response() { Status = "Error", Message = "Out of this furniture" });
            var paymentMethods = await _dbContext.Payments.Select(p => new
            {
                PaymentId = p.PaymentId,
                PaymentMethod = p.PaymentMethod
            }).ToListAsync();
            var deliveryAddress = logginedUser.UserAddresses.FirstOrDefault(ua => ua.AddressType.Equals("DEFAULT"));
            var response = new
            {
                DeliveryAddressId = deliveryAddress.AddressId,
                DeliveryAddress = deliveryAddress.Address.ToString(),
                FurnitureId = orderFurniture.FurnitureId,
                FurnitureName = orderFurniture.Furniture.FurnitureName,
                FurnitureSpecificationId = furnitureSpecificationId,
                FurnitureSpecificationName = orderFurniture.FurnitureSpecificationName,
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
            var logginedUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            //var checkCustomerInfor = CheckCustomerInfor(logginedUser);
            //if (checkCustomerInfor != null) return StatusCode(StatusCodes.Status405MethodNotAllowed,
            //    new Response() { Status = "Error", Message = checkCustomerInfor });
            var userCart = logginedUser.Cart.CartDetails.ToList();
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
            var deliveryAddress = logginedUser.UserAddresses.FirstOrDefault(ua => ua.AddressType.Equals("DEFAULT"));
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
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            //var checkCustomerInfor = CheckCustomerInfor(logginedUser);
            //if (checkCustomerInfor != null) return StatusCode(StatusCodes.Status405MethodNotAllowed,
            //    new Response() { Status = "Error", Message = checkCustomerInfor });
            var customizeFurnitureList = logginedUser.CustomizeFurnitures.ToList();
            var selectedItems = new List<CustomizeFurnitureCheckout>();
            foreach (var id in customizeFurnitureIdList)
            {
                CustomizeFurniture item = customizeFurnitureList.FirstOrDefault(i => i.CustomizeFurnitureId.Equals(id));
                if (item == null) return NotFound($"The customize furniture with id = {id} does not exist in customize furniture list of user");
                if (!item.Result.Status.Equals("Accepted")) return StatusCode(StatusCodes.Status406NotAcceptable, 
                    new Response() { Status = "Error", Message = $"Cannot checkout customize furniture with \"{item.Result.Status}\" status"});
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
            var deliveryAddress = logginedUser.UserAddresses.FirstOrDefault(ua => ua.AddressType.Equals("DEFAULT"));
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
        public async Task<IActionResult> Order(OrderViewModel model)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            //var checkCustomerInfor = CheckCustomerInfor(logginedUser);
            //if (checkCustomerInfor != null) return StatusCode(StatusCodes.Status405MethodNotAllowed,
            //    new Response() { Status = "Error", Message = checkCustomerInfor });
            bool isCustomizeFurnitureOrder = model.Items.FirstOrDefault().ItemId.StartsWith("CF") ? true : false;
            if (isCustomizeFurnitureOrder && model.PaymentId == 1) return BadRequest("Cash payment is not available for customize furniture order");
            if (logginedUser.Point.TotalPoint < model.UsedPoint) return BadRequest("The total point is not enough");
            var deliverAddress = await _dbContext.UserAddresses.FirstOrDefaultAsync(a => a.AddressId == model.AddressId && a.UserId.Equals(logginedUser.Id));
            var newOrder = new Order()
            {
                CustomerId = logginedUser.Id,
                PaymentId = model.PaymentId,
                UsedPoint = model.UsedPoint,
                OrderDate = DateTime.Now,
                DeliveryAddress = deliverAddress.Address.ToString(),
                Note = model.Note == null ? "None" : model.Note,
                Status = "Pending",
                IsPaid = false
            };
            await _dbContext.AddAsync(newOrder);
            await _dbContext.SaveChangesAsync();
            var addedOrder = await _dbContext.Orders.Where(o => o.CustomerId.Equals(logginedUser.Id)).OrderBy(o => o.OrderDate).LastAsync();
            foreach (var item in model.Items)
            {
                
                if (!isCustomizeFurnitureOrder)
                {
                    var itemPrice = _dbContext.FurnitureSpecifications.Find(item.ItemId).Price;
                    var orderItem = new FurnitureOrderDetail()
                    {

                        OrderId = addedOrder.OrderId,
                        FurnitureSpecificationId = item.ItemId,
                        Quantity = item.Quantity,
                        Cost = Math.Round(itemPrice * item.Quantity, 2)
                    };
                    await _dbContext.AddAsync(orderItem);
                }
                else
                {
                    CustomizeFurniture customizeFurniture = await _dbContext.CustomizeFurnitures.FindAsync(item.ItemId);
                    var orderItem = new CustomizeFurnitureOrderDetail()
                    {
                        OrderId = addedOrder.OrderId,
                        CustomizeFunitureId = item.ItemId,
                        Quantity = customizeFurniture.Quantity,
                        Cost = customizeFurniture.Result.ExpectedPrice.Value
                    };
                    await _dbContext.AddAsync(orderItem);
                }              
            }
            await _dbContext.SaveChangesAsync();
            addedOrder.TotalCost = isCustomizeFurnitureOrder ? addedOrder.CustomizeFurnitureOrderDetails.Sum(i => i.Cost) :
                                                               addedOrder.FurnitureOrderDetails.Sum(i => i.Cost);
            _dbContext.Update(addedOrder);
            await _dbContext.SaveChangesAsync();
            if (addedOrder.PaymentId != 1 || isCustomizeFurnitureOrder) return Ok(UrlPayment(addedOrder.PaymentId, addedOrder.OrderId));
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
                                new Response() { Status = "error", Message = $"error payment OrderId={orderId}"});
                            
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
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            Order order = logginedUser.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null) return NotFound($"The order with Id = {orderId} does not exist");
            if (!order.Status.Equals("Preparing") && !order.Status.Equals("Pending")) return BadRequest($"Cannot cancel the order with \"{order.Status}\" status");

            //refund if user pays order via VNPAY
            if (order.IsPaid && order.PaymentId != 1)
            {
                var result = Refund(order);
                if (!result) return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response() { Status = "Error", Message = "An error occur during refund process" });
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
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            var orderList = new List<Order>();
            if (status.Equals("All")) orderList = logginedUser.Orders.ToList();
            else orderList = logginedUser.Orders.Where(o => o.Status.Equals(status)).ToList();
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
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            var responses = logginedUser.Feedbacks.Select(fb => new
            {
                FeedbackId = fb.FeedbackId,
                OrderId = fb.OrderId,
                FurnitureId = fb.FurnitureSpecification.Furniture.FurnitureId,
                FurnitureName = fb.FurnitureSpecification.Furniture.FurnitureName,
                //FurnitureImage = fb.Attachements.Select(a => a.Path),
                Content = fb.Content,
                VoteStar = fb.VoteStar
            });
            if (responses.Count() == 0) return NotFound("No feedback found");
            else return Ok(responses);
        }

        [HttpPost("create-feedback")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> CreationFeedback([FromForm]FeedbackViewModel model)
        {
            //validation
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));

            var order = await _dbContext.Orders.FindAsync(model.OrderId);
            if (order == null) return NotFound("The order does not exist");
            if (!order.Status.Equals("Deliveried")) return StatusCode(StatusCodes.Status406NotAcceptable,
                new Response() { Status = "Error", Message = "Feedback is not available for undelivered order" });

            FurnitureSpecification specification = order.FurnitureOrderDetails.FirstOrDefault(fod => fod.FurnitureSpecificationId.Equals(model.FurnitureSpecificationId)).FurnitureSpecification;
            if (specification == null) return NotFound($"The order does not contains funiture specification with id = {model.FurnitureSpecificationId}");
            var specificationFeedbacks = specification.Feedbacks.ToList();
            if (specificationFeedbacks.FirstOrDefault(fb => fb.OrderId == order.OrderId) != null)
                return StatusCode(StatusCodes.Status406NotAcceptable,
               new Response() { Status = "Error", Message = "This furniture has received feedback" });

            var fileHandle = new FileHandleService();
            //create new feedback 
            var newFeedback = new Feedback()
            {
                CustomerId = logginedUser.Id,
                OrderId = order.OrderId,
                FurnitureSpecificationId = model.FurnitureSpecificationId,
                Content = model.Content,
                VoteStar = model.VoteStar,
                Anonymous = model.Anonymous,
                CreationDate = DateTime.Now
            };
            //calculat average votestar of furniture and update 
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
            await _dbContext.AddAsync(newFeedback);
            _dbContext.Update(furniture);
            await _dbContext.SaveChangesAsync(); 

            //upload image or video
            if (!model.files.IsNullOrEmpty())
            {
                var addedFeedback = await _dbContext.Feedbacks.FirstOrDefaultAsync(fb => fb.OrderId == order.OrderId &&
                                           fb.FurnitureSpecificationId == specification.FurnitureSpecificationId);
                foreach (var file in model.files)
                {
                    var result = fileHandle.UploadFile("Feedback", file);
                    if (result.Equals("Error")) return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response() { Status = "Error", Message = "An error occurs during upload file" });
                    var newFeedbackAttachment = new FeedbackAttachment()
                    {
                        FeedbackId = addedFeedback.FeedbackId,
                        AttachmentName = file.FileName,
                        Path = result,
                        Type = fileHandle.ImageOrVideo(file)
                    };
                    await _dbContext.AddAsync(newFeedbackAttachment);
                   
                }
                await _dbContext.SaveChangesAsync();
            }

           
            return Ok("Create feedback successfully");
        }

                                                                            //CUSTOMIZE FURNITURE

        //GET
        [HttpGet("customize-furnitures")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetCustomizeFurniture(string status)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            List<CustomizeFurniture> customizeFurnitures = new List<CustomizeFurniture>();
            if (status.Equals("All")) customizeFurnitures = logginedUser.CustomizeFurnitures.ToList();
            else customizeFurnitures = logginedUser.CustomizeFurnitures.Where(cf => cf.Result.Status.Equals(status)).ToList();
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
                        Path = a.Path
                    }),
                    Videos = cf.Attachments.Where(a => a.Type.Equals("Videos")).Select(a => new
                    {
                        AttachmentName = a.AttachmentName,
                        Path = a.Path
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
                    new Response() { Status = "Error", Message = "An error occurs during fetch data" });
            }

        }

        //CREATE
        [HttpPost("customize-furnitures/create")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> CreateCustomizeFurniture([FromForm] CustomizeFurnitureViewModel userInput)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            //var checkCustomerInfor = CheckCustomerInfor(logginedUser);
            //if (checkCustomerInfor != null) return StatusCode(StatusCodes.Status405MethodNotAllowed,
            //    new Response() { Status = "Error", Message = checkCustomerInfor });

            // tao customize furniture de shop owner duyet
            var newCustomizeFurniture = new CustomizeFurniture()
            {
                CustomizeFurnitureId = "CF-"+Guid.NewGuid().ToString().ToUpper(),
                CustomerId = logginedUser.Id,
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
            if (userInput.Attachments.IsNullOrEmpty())
            {
                var fileHandle = new FileHandleService();
                foreach (var file in userInput.Attachments)
                {
                    var result = fileHandle.UploadFile("CustomizeFurniture", file);
                    if (result.Equals("Error")) return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response() { Status = "Error", Message = "An error occurs during upload file" });
                    var newAttachment = new CustomizeFurnitureAttachment()
                    {
                        CustomizeFurnitureId = newCustomizeFurniture.CustomizeFurnitureId,
                        AttachmentName = file.FileName,
                        Path = result,
                        Type = fileHandle.ImageOrVideo(file)
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
            var logginedUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            CustomizeFurniture customizeFurniture = logginedUser.CustomizeFurnitures.FirstOrDefault(cf => cf.CustomizeFurnitureId.Equals(userInput.CustomizeFurnitureId));
            if (customizeFurniture == null) return NotFound("Customize furniture not found");
            if (!customizeFurniture.Result.Status.Equals("Pending")) return StatusCode(StatusCodes.Status406NotAcceptable,
                new Response() { Status = "Error", Message = "The customize furniture can only be changed while in the pending status" });

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
                var fileHandle = new FileHandleService();
                foreach (var attachment in customizeFurniture.Attachments)
                {
                    fileHandle.DeleteFile(attachment.Path);
                    _dbContext.Remove(attachment);
                };
                await _dbContext.SaveChangesAsync();
                foreach (var file in userInput.Attachments)
                {
                    var result = fileHandle.UploadFile("CustomizeFurniture", file);
                    if (result.Equals("Error")) return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response() { Status = "Error", Message = "An error occurs during upload file" });
                    var newAttachment = new CustomizeFurnitureAttachment()
                    {
                        CustomizeFurnitureId  = customizeFurniture.CustomizeFurnitureId,
                        AttachmentName = file.FileName,
                        Path = result,
                        Type = fileHandle.ImageOrVideo(file)
                    };
                    await _dbContext.AddAsync(newAttachment);
                }
                await _dbContext.SaveChangesAsync();
            }
            return Ok("Update customize furnituer successfully"); 
        }

        //DELETE
        [HttpDelete("customize-furnitures/remove")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> DeleteCustomizeFurniture(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
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
                    new Response() { Status = "Error", Message = $"An error occurred while removing custom furniture {ex.Message}" });

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
            var logginedUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            status = status.IsNullOrEmpty() ? "All" : status;
            if (!status.Equals("Pending") && !status.Equals("Accepted") && !status.Equals("Not accepted") && !status.Equals("All")) return BadRequest("The status must be \"Pending\", \"Accepted\", \"Not accepted\" and \"All\"");
            var warrantyList = logginedUser.Warranties.ToList();
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
                        Path = a.Path
                    }),
                    Videos = w.Attachments.Where(a => a.Type.Equals("Videos")).Select(a => new
                    {
                        FileName = a.AttachmentName,
                        Path = a.Path
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
            var logginedUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            var orderExist = logginedUser.Orders.FirstOrDefault(w => w.OrderId == userInput.OrderId);
            if (orderExist == null) return NotFound($"The order witd id = {userInput.OrderId} does not exist in the ordered list of customer");
            if (!orderExist.Status.Equals("Deliveried")) return BadRequest($"Not allow to create warranty when order is in {orderExist.Status} status");
            
            Warranty newWarranty = new Warranty()
            {
                UserId = logginedUser.Id,
                OrderId = userInput.OrderId,
                WarrantyReasons = userInput.WarrantyReasons,
                Status = "Pending"
            };
            await _dbContext.AddAsync(newWarranty);
            await _dbContext.SaveChangesAsync();
            if (!userInput.UploadFiles.IsNullOrEmpty())
            {
                var fileHandle = new FileHandleService();
                foreach (var file in userInput.UploadFiles)
                {
                    var result = fileHandle.UploadFile("Warranty", file);
                    if (result.Equals("Error")) return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response() { Status = "Error", Message = "An error occurs during upload file" });
                    var newAttachment = new WarrantyAttachment()
                    {
                        WarrantyId = newWarranty.WarrantyId,
                        AttachmentName = file.FileName,
                        Path = result,
                        Type = fileHandle.ImageOrVideo(file)
                    };
                    await _dbContext.AddAsync(newAttachment);
                }
                await _dbContext.SaveChangesAsync();
            }
            return StatusCode(StatusCodes.Status201Created,
                new Response() { Status = "Success", Message = "Create warranty successfully" });
        }

        //UPDATE
        [HttpPut("warranties/update")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> UpdateWarranty([FromForm] EditWarrantyViewModel userInput)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            var editWarranty = logginedUser.Warranties.FirstOrDefault(w => w.WarrantyId == userInput.WarrantyId);
            if (editWarranty == null) return NotFound($"warranty claim with id = {userInput.WarrantyId} was not found in in the customer's warranty claim list");
            
            editWarranty.WarrantyReasons = userInput.WarrantyReasons;

            _dbContext.Update(editWarranty);
            await _dbContext.SaveChangesAsync();
            if (!userInput.UploadFiles.IsNullOrEmpty())
            {
                var fileHandle = new FileHandleService();
                foreach (var attachment in editWarranty.Attachments)
                {
                    fileHandle.DeleteFile(attachment.Path);
                    _dbContext.Remove(attachment);
                };
                foreach (var file in userInput.UploadFiles)
                {
                    var result = fileHandle.UploadFile("Warranty", file);
                    if (result.Equals("Error")) return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response() { Status = "Error", Message = "An error occurs during upload file" });
                    var newAttachment = new WarrantyAttachment()
                    {
                        WarrantyId = editWarranty.WarrantyId,
                        AttachmentName = file.FileName,
                        Path = result,
                        Type = fileHandle.ImageOrVideo(file)
                    };
                    _dbContext.Update(newAttachment);
                }
                await _dbContext.SaveChangesAsync();
            }
            return StatusCode(StatusCodes.Status201Created,
                new Response() { Status = "Success", Message = "Edit warranty successfully" });
        }
        
        //DELETE
        [HttpDelete("warranties/remove/{id}")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> RemoveWarranty([Required]int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            Warranty removeWarranty = logginedUser.Warranties.FirstOrDefault(w => w.WarrantyId == id);
            if (removeWarranty == null) return NotFound($"warranty claim with id = {id} was not found in in the customer's warranty claim list");

            try
            {
                _dbContext.RemoveRange(removeWarranty.Attachments);
                await _dbContext.SaveChangesAsync();
                _dbContext.Remove(removeWarranty);
                await _dbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status204NoContent,
                    new Response() { Status = "Success", Message = "Remove warranty claim successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   new Response() { Status = "Error", Message = "An error occurs when removing warranty claim" });
            }

        }

        

            ////SUPPORTED METHODs
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool Refund(Order order)
        {
            var transition = GetTransition(order);
            if (transition == null) return false;
            var vnp_Api = _config["VNPAY:Api"];
            var vnp_HashSecret = _config["VNPAY:HashSecret"]; //Secret KEy
            var vnp_TmnCode = _config["VNPAY:TmnCode"]; // Terminal Id
            
            var vnp_RequestId = DateTime.Now.Ticks.ToString(); //Mã hệ thống merchant tự sinh ứng với mỗi yêu cầu hoàn tiền giao dịch. Mã này là duy nhất dùng để phân biệt các yêu cầu truy vấn giao dịch. Không được trùng lặp trong ngày.
            var vnp_Version = VnPayService.VERSION; //2.1.0
            var vnp_Command = "refund";
            var vnp_TransactionType = "02";
            var vnp_Amount = transition.Amount;
            var vnp_TxnRef = transition.TxnRef; // Mã giao dịch thanh toán tham chiếu
            var vnp_OrderInfo = "Hoan tien giao dich:" + order.OrderId;
            var vnp_TransactionNo = transition.TransactionNo;
            var vnp_TransactionDate = transition.PayDate;
            var vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            var vnp_CreateBy = order.Customer.UserName;
            string ipAddress = Response.HttpContext.Connection.RemoteIpAddress.ToString();
            if (ipAddress.Equals("::1")) ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
            var vnp_IpAddr = ipAddress;

            var signData = vnp_RequestId + "|" + vnp_Version + "|" + vnp_Command + "|" + vnp_TmnCode + "|" + vnp_TransactionType + "|" + vnp_TxnRef + "|" + vnp_Amount + "|" + vnp_TransactionNo + "|" + vnp_TransactionDate + "|" + vnp_CreateBy + "|" + vnp_CreateDate + "|" + vnp_IpAddr + "|" + vnp_OrderInfo;
            var vnp_SecureHash = Utils.HmacSHA512(vnp_HashSecret, signData);

            var rfData = new
            {
                vnp_RequestId = vnp_RequestId,
                vnp_Version = vnp_Version,
                vnp_Command = vnp_Command,
                vnp_TmnCode = vnp_TmnCode,
                vnp_TransactionType = vnp_TransactionType,
                vnp_TxnRef = vnp_TxnRef,
                vnp_Amount = vnp_Amount,
                vnp_OrderInfo = vnp_OrderInfo,
                vnp_TransactionNo = vnp_TransactionNo,
                vnp_TransactionDate = vnp_TransactionDate,
                vnp_CreateBy = vnp_CreateBy,
                vnp_CreateDate = vnp_CreateDate,
                vnp_IpAddr = vnp_IpAddr,
                vnp_SecureHash = vnp_SecureHash

            };
            var jsonData = JsonConvert.SerializeObject(rfData);
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(vnp_Api);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(jsonData);
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var strData = "";
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                strData = streamReader.ReadToEnd();
            }
            if (!strData.Contains("\"vnp_ResponseCode\":\"00\"")) return false;
            return true;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public VnPayTransition GetTransition(Order order)
        {
            var vnp_Api = _config["VNPAY:Api"];
            var vnp_HashSecret = _config["VNPAY:HashSecret"]; //Secret KEy
            var vnp_TmnCode = _config["VNPAY:TmnCode"]; // Terminal Id

            var vnp_RequestId = DateTime.Now.Ticks.ToString(); //Mã hệ thống merchant tự sinh ứng với mỗi yêu cầu truy vấn giao dịch. Mã này là duy nhất dùng để phân biệt các yêu cầu truy vấn giao dịch. Không được trùng lặp trong ngày.
            var vnp_Version = VnPayService.VERSION; //2.1.0
            var vnp_Command = "querydr";
            var vnp_TxnRef = order.OrderId.ToString(); // Mã giao dịch thanh toán tham chiếu
            var vnp_OrderInfo = "Payment orders:" + order.OrderId;
            var vnp_TransactionDate = order.OrderDate.ToString("yyyyMMdd");
            var vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");

            string ipAddress = Response.HttpContext.Connection.RemoteIpAddress.ToString();
            if (ipAddress.Equals("::1")) ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
            var vnp_IpAddr = ipAddress;

            var signData = vnp_RequestId + "|" + vnp_Version + "|" + vnp_Command + "|" + vnp_TmnCode + "|" + vnp_TxnRef + "|" + vnp_TransactionDate + "|" + vnp_CreateDate + "|" + vnp_IpAddr + "|" + vnp_OrderInfo;
            var vnp_SecureHash = Utils.HmacSHA512(vnp_HashSecret, signData);

            var qdrData = new
            {
                vnp_RequestId = vnp_RequestId,
                vnp_Version = vnp_Version,
                vnp_Command = vnp_Command,
                vnp_TmnCode = vnp_TmnCode,
                vnp_TxnRef = vnp_TxnRef,
                vnp_OrderInfo = vnp_OrderInfo,
                vnp_TransactionDate = vnp_TransactionDate,
                vnp_CreateDate = vnp_CreateDate,
                vnp_IpAddr = vnp_IpAddr,
                vnp_SecureHash = vnp_SecureHash

            };
            var jsonData = JsonConvert.SerializeObject(qdrData);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(vnp_Api);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(jsonData);
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var strData = "";
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                strData = streamReader.ReadToEnd();
            }
            if (!strData.Contains("\"vnp_ResponseCode\":\"00\"")) return null;
            JObject data = JObject.Parse(strData);
            var vnPayTransition = new VnPayTransition()
            {
                    TxnRef = (string) data["vnp_TxnRef"],
                    Amount = (string)data["vnp_Amount"],
                    OrderInfo = (string)data["vnp_OrderInfo"],
                    BankCode = (string)data["vnp_BankCode"],
                    PayDate = (string)data["vnp_PayDate"],
                    TransactionNo = (string)data["vnp_TransactionNo"],
                    TransactionType = (string)data["vnp_TransactionType"],
                    TransactionStatus = (string)data["vnp_TransactionStatus"],
            };
            return vnPayTransition;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string UrlPayment(int typePayment, int orderId)
        {

            var order = _dbContext.Orders.FirstOrDefault(o => o.OrderId == orderId);

            //Get Config Info
            string vnp_Returnurl = _config["VNPAY:ReturnUrl"]; //URL nhan ket qua tra ve 
            string vnp_Url = _config["VNPAY:Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = _config["VNPAY:TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = _config["VNPAY:HashSecret"]; //Secret Key
            //Build URL for VNPAY
            VnPayService vnpay = new VnPayService();
            vnpay.AddRequestData("vnp_Version", VnPayService.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            if (!order.CustomizeFurnitureOrderDetails.IsNullOrEmpty()) 
            {
                vnpay.AddRequestData("vnp_Amount", (order.TotalCost * 100000/2).ToString()); // thanh toán trước 50% nếu là đơn customize
            } else
            {
                vnpay.AddRequestData("vnp_Amount", (order.TotalCost * 100000).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            }
            var payment = _dbContext.Payments.Find(typePayment);
            vnpay.AddRequestData("vnp_BankCode", payment.PaymentMethod);
            vnpay.AddRequestData("vnp_CreateDate", order.OrderDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            string ipAddress = Response.HttpContext.Connection.RemoteIpAddress.ToString();
            if (ipAddress.Equals("::1")) ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
            vnpay.AddRequestData("vnp_IpAddr", ipAddress);
            vnpay.AddRequestData("vnp_Locale", "en");
            vnpay.AddRequestData("vnp_OrderInfo", "Payment orders:" + order.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.OrderId.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày
            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return paymentUrl;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<bool> VerifyPhoneNum(string phoneNums, string totpCode)
        {
            byte[] secretKey = Encoding.ASCII.GetBytes(phoneNums);
            var otp = new Totp(secretKey, step: 300, totpSize: 6);
            var window = new VerificationWindow(previous: 1, future: 1);
            long timeStepMatched;
            var result = otp.VerifyTotp(DateTime.Now, totpCode, out timeStepMatched, window);
            if (!result)
            {
                return false;
            }
            return true;
        }
        
        [ApiExplorerSettings(IgnoreApi = true)]
        private JwtSecurityToken GenerateJWTToken(User user, IList<string> roles)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _config["JWT:ValidIssuer"],
                audience: _config["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(2),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return token;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<bool> CreateUserInfor(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            var userPoint = new Models.Purchase.Point()
            {
                CustomerId = user.Id,
                User = user,
                Description = "Create account successfully +5 point",
                TotalPoint = 5,
                History = DateTime.Now
            };
            var announcement = new Announcement()
            {
                UserId = user.Id,
                User = user,
                Title = "Welcome",
                Content = "Welcome to Furniture Shopping Online, wish you have a great shopping experience!",
                CreationDate = DateTime.Now
            };
            var cart = new Cart()
            {
                CustomerId = user.Id
            };
            await _dbContext.AddRangeAsync(announcement, userPoint, cart);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string CheckCustomerInfor(User customer)
        {
            if (customer.UserAddresses.Count == 0 || customer.UserAddresses == null) return "The customer must add address to use this fuction";
            if (customer.PhoneNumber == null || !customer.PhoneNumberConfirmed) return "The customer must add phone number and verify it to use this fuction";
            return null;
        }
    }
}
