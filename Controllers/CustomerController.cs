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
using OnlineShopping.ViewModels.Furniture;
using Org.BouncyCastle.Crypto.Operators;
using Microsoft.AspNet.SignalR.Hosting;

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

        [HttpGet("test")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> TestAuthorize()
        {
            return StatusCode(StatusCodes.Status406NotAcceptable,
                    new Response() { Status = "Error", Message = "Testing" });
        }

        //login/register/ authorization/ authentication
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
                UserName = userInfor.Username,
                PhoneNumber = userInfor.PhoneNumbers,
                Email = userInfor.Email,
                CreationDate = DateTime.Now,
                Status = "Activated",
                TwoFactorEnabled = false,
                Announcements = new List<Announcement>(),
                Feedbacks = new List<Feedback>(),
                Orders = new List<Order>(),
                UserAddresses = new List<UserAddress>(),
                CustomizeFurniture = new List<CustomizeFurniture>(),
                WarrantySchedules = new List<WarrantySchedule>(),            
            };
            var result = await _userManager.CreateAsync(newUser, userInfor.Password);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status409Conflict,
                    new Response() { Status = "Error", Message = result.ToString() });
            }
            await CreateUserInfor(newUser.Email);
            await _userManager.AddToRoleAsync(newUser, "CUSTOMER");         
            //var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            //var confirmationLink = Url.Action(nameof(ConfirmEmail),
            //    "Customer", new { token, email = newUser.Email }, Request.Scheme);
            //var message = new Message(new string[] { newUser.Email! },
            //    "Confirmation email link", $"Please click to the following Url to verify your email: \n {confirmationLink!}");
            //_emailService.SendEmail(message);       
            return StatusCode(StatusCodes.Status201Created,
                    new Response { Status = "Success", Message = $"Account created & a confirmation email sent to {newUser.Email} successFully" });
        }
        
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<bool> CreateUserInfor(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;
            var userCart = new Cart()
            {
                CustomerId = user.Id,
                Customer = user,
                CartDetails = new List<CartDetail>()
            };
            var userWishList = new WishList()
            {
                CustomerId = user.Id,
                Customer = user,
                
            };
            var userPoint = new Point()
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
            user.Cart = userCart;
            user.WishList = userWishList;
            user.Point = userPoint;
            user.Announcements.Add(announcement);
            await _dbContext.AddRangeAsync(userCart, userWishList, announcement,userPoint);
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var userExist = await _userManager.FindByEmailAsync(email);
            if (userExist != null)
            {
                var result = await _userManager.ConfirmEmailAsync(userExist, token);
                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response() { Status = "Error", Message = result.ToString() });
                }

                return StatusCode(StatusCodes.Status200OK,
                        new Response() { Status = "Success", Message = $"Confim email sucessfully!" });
            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response() { Status = "Error", Message = "This user is not exist" });
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<bool> SendOTPConfirmation(string email, string phoneNums)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return false;
            }
            byte[] secretKey = Encoding.ASCII.GetBytes(phoneNums);          
            var otp = new Totp(secretKey, step: 300, totpSize: 6);
            var totpCode = otp.ComputeTotp(DateTime.Now);
            var sms = new Sms(new string[] { phoneNums }, $"The OTP to confirm your phone number: {totpCode}");
            _smsService.SendSMS(sms);
            return true;
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
            var externalUserInfor = await _signInManager.GetExternalLoginInfoAsync();
            var externalEmail = externalUserInfor.Principal.FindFirstValue(ClaimTypes.Email);
            var userWithExternalMail = await _userManager.FindByEmailAsync(externalEmail);
            
            //truong hop da co tai khoan trong db
            if(userWithExternalMail != null)
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
            //truong hop chua cho user trong db
            Random rnd = new Random();
            var newUser = new User()
            {
                UserName = $"user@{rnd.Next(10000, 99999)}{rnd.Next(10000, 99999)}",
                FirstName = externalUserInfor.Principal.FindFirstValue(ClaimTypes.GivenName) == null ? 
                     "New" : externalUserInfor.Principal.FindFirstValue(ClaimTypes.GivenName),
                LastName = externalUserInfor.Principal.FindFirstValue(ClaimTypes.Surname) == null ? 
                     "User" : externalUserInfor.Principal.FindFirstValue(ClaimTypes.Surname),           
                DoB = externalUserInfor.Principal.FindFirstValue(ClaimTypes.DateOfBirth) == null ?
                     DateTime.Now : DateTime.Parse(externalUserInfor.Principal.FindFirstValue(ClaimTypes.DateOfBirth)),
                Gender = externalUserInfor.Principal.FindFirstValue(ClaimTypes.Gender) == null ?
                     "Secret" : externalUserInfor.Principal.FindFirstValue(ClaimTypes.Gender),
                PhoneNumber = externalUserInfor.Principal.FindFirstValue(ClaimTypes.MobilePhone) == null ? 
                     "Empty" : externalUserInfor.Principal.FindFirstValue(ClaimTypes.MobilePhone),
                Email = externalEmail,
                EmailConfirmed = true,
                CreationDate = DateTime.Now,
                Status = "Activated",
                TwoFactorEnabled = false,
                Announcements = new List<Announcement>(),
                Feedbacks = new List<Feedback>(),
                Orders = new List<Order>(),
                UserAddresses = new List<UserAddress>(),
                CustomizeFurniture = new List<CustomizeFurniture>(),
                WarrantySchedules = new List<WarrantySchedule>(),           
            };          
            var result = await _userManager.CreateAsync(newUser);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser,"CUSTOMER");
                result = await _userManager.AddLoginAsync(newUser, externalUserInfor);
                await _signInManager.SignInAsync(newUser, isPersistent: false, externalUserInfor.LoginProvider);
                await CreateUserInfor(newUser.Email);
                var jwtToken = GenerateJWTToken(newUser, await _userManager.GetRolesAsync(newUser));
                //return the token
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    expiration = jwtToken.ValidTo
                });
            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                       new Response() { Status = "Error", Message = "Can not add login with external account" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUser loginModel)
        {
            var logginedUser = await _userManager.FindByEmailAsync(loginModel.Email);
            var passwordChecker = await _userManager.CheckPasswordAsync(logginedUser, loginModel.Password);
            //checking if the user existed and password was valid
            if (logginedUser == null || !passwordChecker)
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
                var message = new Message(new string[] { user.Email }, "Reset Password OTP", forgotPasswordLink);
                _emailService.SendEmail(message);
                return StatusCode(StatusCodes.Status200OK,
                   new Response { Status = "Success", Message = $"User created & Email Sent to {user.Email} Successfully" });
            }
            return StatusCode(StatusCodes.Status400BadRequest,
                   new Response { Status = "Error", Message = "Incorrect email. Please try again!" });
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

        [HttpGet("reset-password")]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            var model = new ResetPassword { Email = email, Token = token };

            return Ok(new { model });
        }

        //furniture
        [HttpGet("furnitures")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllFurniture()
        {
            var furnitures = await _dbContext.Furnitures.ToListAsync();
            return Ok(furnitures);
        }


        [HttpGet("furnitures/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFurnitureSpecificationById(int id)
        {
            var furnitureSpecification = await _dbContext.FurnitureSpecifications.Include(fs => fs.Wood).Include(fs => fs.Color)
                .Where(fs => fs.FurnitureId == id).ToListAsync();
          
            if (furnitureSpecification.Count == 0)
            {
                return NotFound();
            }
            var responses = new List<FurnitureSpecificationViewModel>();
            foreach (var fs in furnitureSpecification)
            {
                fs.Furniture = await _dbContext.Furnitures.FindAsync(fs.FurnitureId);
                fs.Wood = await _dbContext.Woods.FindAsync(fs.WoodId);
                fs.Color = await _dbContext.Colors.FindAsync(fs.ColorId);
                fs.Description = "Test changes";
                await _dbContext.SaveChangesAsync();

                var response = new FurnitureSpecificationViewModel()
                {
                    FurnitureSpecificationId = fs.FurnitureSpecificationId,
                    FurnitureId = fs.FurnitureId,
                    FurnitureName = fs.Furniture.FurnitureName,
                    Height = fs.Height,
                    Width = fs.Width,
                    Length = fs.Length,
                    Color = fs.Color.ColorName,
                    Wood = fs.Wood.WoodType,
                    Description = fs.Description
                };
                responses.Add(response);
            }
            return Ok(responses);           
        }

        [HttpGet("search")]
        [AllowAnonymous] 
        public async Task<IActionResult> SearchFurniture(string keyword)
        {
            var result = await _dbContext.Furnitures.Where(f => f.FurnitureName.Contains(keyword)).ToListAsync();
            if (result.Count == 0) 
            {
                return NotFound();
            }
            return Ok(result);
        }

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
                FurnitureName = cd.FurnitureSpecifition.Furniture.FurnitureName,
                FurnitureSpecificationName = cd.FurnitureSpecifition.FurnitureSpecificationName,
                Quantity = cd.Quantity,
                Cost = cd.Quantity * cd.FurnitureSpecifition.Price
            }); 
            return Ok(usercCart);
        }

        [HttpPost("add-to-cart")]
        [AllowAnonymous]
        public async Task<IActionResult> AddToCart(int furnitureSpecificationId, int quantity)
        {
            if (furnitureSpecificationId == null || quantity == null) return BadRequest();
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            var furnitureSpecification = await _dbContext.FurnitureSpecifications.FindAsync(furnitureSpecificationId);
            if (furnitureSpecification == null) return NotFound("The furniture not found");
            //kiem tra san san pham da co trong cart cua nguoi dung chua
            var cartDetailExist = await _dbContext.CartDetails.FirstOrDefaultAsync(cd => cd.CartId == logginedUser.Cart.CartId
                                           && cd.FurnitureSpecificationId == furnitureSpecificationId);
            if (cartDetailExist != null)
            {
                if (quantity == 0)
                {
                    _dbContext.Remove(cartDetailExist);
                    logginedUser.Cart.CartDetails.Remove(cartDetailExist);
                    _dbContext.Update(logginedUser);
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
                {
                    CartId = 2,
                    Cart = logginedUser.Cart,
                    FurnitureSpecificationId = furnitureSpecificationId,
                    Quantity = quantity,
                    FurnitureSpecifition = furnitureSpecification
                };
                await _dbContext.AddAsync(newCartDetail);
                logginedUser.Cart.CartDetails.Add(newCartDetail);
            }
            _dbContext.Update(logginedUser);
            await _dbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status200OK,
                   new Response() { Status = "Success", Message = "Add the furniture to cart suuccessfully" });
        }

        [HttpPost("toggle-wishlist")]
        [AllowAnonymous]
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

        [HttpGet("furniture-filter")]
        [AllowAnonymous]
        public async Task<IActionResult> FurnitureFilter(double minCost, double maxCost, string category, 
            int star, string appropriate, string collection)
        {
            if (minCost > maxCost) return BadRequest("The maximum price must be greater or equal minimum price");
            maxCost = maxCost == 0 ? double.MaxValue : maxCost;
            var furnitures = await _dbContext.FurnitureSpecifications.Where(fs => fs.Price >= minCost && fs.Price <= maxCost).ToListAsync();
            //if (star != 0) response = response.Where(fs => fs.Furniture.VoteStar == star).ToList();
            //if (category != null) response = response.Where(fs => fs.Furniture.Category.CategoryName == category).ToList();
            return Ok(furnitures);
            }

        // user infor
        [HttpGet("customer-address")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetCustomerContact()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            var AddressesList = await _dbContext.UserAddresses.Where(a => a.UserId.Equals(logginedUser.Id)).Select(a => new
            {
                Id = a.Address.AddressId,
                Street = a.Address.Street,
                Commune = a.Address.Commune,
                District = a.Address.District,
                Provine = a.Address.Provine
            }                        
            ).ToListAsync();         
            return Ok(AddressesList);
        }
        
        [HttpGet("customer-phonenumber")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetCustomerPhoneNum()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            return Ok(logginedUser.PhoneNumber);  
        }

        [HttpGet("change-customer-phonenumber")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetPhoneNumChangeOTP(string phoneNum)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");           
            var result = await SendOTPConfirmation(email, phoneNum);
            if (!result) return BadRequest();
            return StatusCode(StatusCodes.Status102Processing,
                new Response() { Status = "Success", Message = $"System has sent OTP to {phoneNum}"});
        }

        [HttpPost("update-customer-phonenumber")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> ChangeCustomerPhoneNum(string OTP, string phoneNum)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            var result = await VerifyPhoneNum(phoneNum, OTP);
            if (!result)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new Response() { Status = "Error", Message = "The OTP is incorrect" });
            }
            logginedUser.PhoneNumber = phoneNum;
            logginedUser.PhoneNumberConfirmed = true;
            _dbContext.Update(logginedUser);
            await _dbContext.SaveChangesAsync();
            return Ok("Change phone number successfully");
        }
        [HttpPost("add-customer-address")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult>AddCustomerContact(string street, string commune, string district, string provine, string type)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            var newAddress = new Models.Address()
            {
                Street = street,
                Commune = commune,
                District = district,
                Provine = provine,
                AddressOwner = "USER",
                UserAddresses = new List<UserAddress>()
            };
            await _dbContext.AddAsync(newAddress);
            await _dbContext.SaveChangesAsync();
            var addedAddress = _dbContext.Addresses.OrderBy(a => a.AddressId).Last();
            var newUserAddress = new UserAddress()
            {
                AddressId = addedAddress.AddressId,
                UserId = logginedUser.Id,
                AddressType = type,
                User = logginedUser,
                Address = addedAddress
            };
            addedAddress.UserAddresses.Add(newUserAddress);
            logginedUser.UserAddresses.Add(newUserAddress);
            _dbContext.UpdateRange(addedAddress, logginedUser);
            await _dbContext.SaveChangesAsync();
            return Ok("Add address successfully");
        }
        
        [HttpPost("update-customer-address")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> UpdateCustomerContact(int AddressId, string PhoneNum)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            return Ok();
        }

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

        [HttpPost("update-infor")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> UpdateInfor(string firstName, string lastName, string gender, DateTime doB, IFormFile image)
        {
          
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            logginedUser.FirstName = firstName == null ? logginedUser.FirstName : firstName;
            logginedUser.LastName = lastName == null ? logginedUser.LastName : lastName; ;
            logginedUser.DoB = doB == null ? logginedUser.DoB : doB; ;
            logginedUser.Gender = gender == null ? logginedUser.Gender : gender; ;
            if (image != null)
            {                
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "Assets\\Images\\UserImages");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    System.IO.File.Delete(logginedUser.Avatar);              
                    string fileNameWithPath = Path.Combine(path, image.FileName);
                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                         image.CopyTo(stream);
                    }                   
                    logginedUser.Avatar = fileNameWithPath;
                    _dbContext.Update(logginedUser);
                    await _dbContext.SaveChangesAsync();
                
            }
            _dbContext.Update(logginedUser);
            await _dbContext.SaveChangesAsync();
            return Ok("Update user information successfully");
        }



    }
}
