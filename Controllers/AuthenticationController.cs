using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OnlineShopping.Data;
using OnlineShopping.Hubs;
using OnlineShopping.Hubs.Models;
using OnlineShopping.Libraries.Models;
using OnlineShopping.Libraries.Services;
using OnlineShopping.Models;
using OnlineShopping.ViewModels;
using OnlineShopping.ViewModels.Login;
using OnlineShopping.ViewModels.User;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OnlineShopping.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
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
        private readonly IHubContext<SignalHub> _hubContext;

        public AuthenticationController(ApplicationDbContext dbContext, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager,
            IConfiguration config, IEmailService emailService, ISMSService smsService, IFirebaseService firebaseService, IProjectHelper projectHelper, IHubContext<SignalHub> hubContext)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _config = config;
            _emailService = emailService;
            _smsService = smsService;
            _firebaseService = firebaseService;
            _projectHelper = projectHelper;
            _hubContext = hubContext;
        }

        [HttpGet("abc")]
        [AllowAnonymous]
        public async Task<IActionResult> SignalRTets()
        {
            var data = new JwtToken("testing123", DateTime.Now);
            _hubContext.Clients.All.SendAsync("ReceiveJWTToken", data);
            return Ok("Done");
        }



        [HttpPost("create-account/{roleId}")]    
        public async Task<IActionResult> RegisterCustomerAccount([FromBody] RegisterCustomerAccount userInfor, [FromRoute] string roleId)
        {
            var roleExist = await _roleManager.FindByIdAsync(roleId);
            if (roleExist == null) return BadRequest(" the role with id = " + roleId + " was not found");

            var mailChecker = await _userManager.FindByEmailAsync(userInfor.Email);
            if (mailChecker != null)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable,
                    new Response("Error", "The email already used!"));
            }

            var phoneChecker = _userManager.Users.FirstOrDefault(u => u.PhoneNumber.Equals(userInfor.PhoneNumbers));
            if (phoneChecker != null)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable,
                    new Response("Error", "The phone numbers already used!"));
            };


            if (!roleExist.Id.Equals("1"))
            {
                string email = User.FindFirstValue(ClaimTypes.Email);
                if (email == null) return Unauthorized();
                var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
                var roles = await _userManager.GetRolesAsync(loggedInUser);
                if (!roles.Contains("SHOP_OWNER")) return Unauthorized();
            }


            var newUser = new User()
            {
                FirstName = userInfor.FirstName,
                LastName = userInfor.LastName,
                DoB = userInfor.DoB,
                Gender = userInfor.Gender,
                UserName = userInfor.Email,
                PhoneNumber = userInfor.PhoneNumbers,
                Email = userInfor.Email,
                Avatar = "",
                CreationDate = DateTime.Now,
                LatestUpdate = DateTime.Now,
                IsActivated = true,
                TwoFactorEnabled = false,
                Spent = 0,
                Debit = 0
            };

            var addUserResult = await _userManager.CreateAsync(newUser, userInfor.Password);
            if (!addUserResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status409Conflict,
                    new Response("Error", addUserResult.ToString()));
            }
            var addUserRoleResult = await _userManager.AddToRoleAsync(newUser, roleExist.Name);
            if (!addUserRoleResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status409Conflict,
                    new Response("Error", addUserRoleResult.ToString()));
            }

            //if role is not customer
            if (!roleExist.Id.Equals("1")) {
                var response = new
                {
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName,
                    DoB = newUser.DoB,
                    Gender = newUser.Gender,
                    PhoneNumber = newUser.PhoneNumber,
                    Email = newUser.Email,
                    Avatar = newUser,
                    CreationDate = newUser.CreationDate,
                    LatestUpdate = newUser.LatestUpdate,
                    IsActivated = newUser,
                    TwoFactorEnabled = newUser
                };
                return Created("", response);
            }

            await _projectHelper.CreateUserInfor(newUser.Email);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var confirmationLink = Url.Action("ConfirmEmail",
                "Authentication", new { token, email = newUser.Email }, Request.Scheme);
            var message = new Message(new string[] { newUser.Email! },
                "Confirmation email link", $"Please click to the following Url to verify your email: \n {confirmationLink!}");
            _emailService.SendEmail(message);
            return StatusCode(StatusCodes.Status201Created,
                    new Response("Success", $"Account created & a confirmation email sent to {newUser.Email} successFully"));
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return Unauthorized(new Response("Error", "No account matches email"));

            var result = _userManager.ConfirmEmailAsync(user, token);
            if (!result.Result.Succeeded) return Unauthorized(new Response("Error", "No account matches email"));

            //return Ok(new Response("Success", "success email confirmation"));
            return RedirectPermanent("http://localhost:8080");
        }

        [HttpGet("signin-google")]
        public IActionResult SignInGoogle()
        {
            var redirectUri = Url.Action(nameof(HandleGoogleCallback), "Authentication");
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
                       new Response("Error", "An error occurs during login"));
            }
            try
            {
                //truong hop da co tai khoan trong local db
                if (userWithExternalMail != null)
                {
                    //confirm luon email
                    if (!userWithExternalMail.EmailConfirmed)
                    {
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(userWithExternalMail);
                        await _userManager.ConfirmEmailAsync(userWithExternalMail, token);
                    }
                    // Thực hiện liên kết info và user

                    var resultAdd = await _userManager.AddLoginAsync(userWithExternalMail, externalUserInfor);
                    if (resultAdd.Succeeded || resultAdd.ToString().Equals("Failed : LoginAlreadyAssociated"))
                    {
                        // Thực hiện login    
                        if (!userWithExternalMail.IsActivated) return StatusCode(StatusCodes.Status401Unauthorized, new Response("Error", "Inactive accounts are not allowed to log in"));

                        await _signInManager.SignInAsync(userWithExternalMail, isPersistent: false);
                        var jwtToken = _projectHelper.GenerateJWTToken(userWithExternalMail, await _userManager.GetRolesAsync(userWithExternalMail));
                        var response = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                            expiration = jwtToken.ValidTo
                        };
                        _hubContext.Clients.All.SendAsync("ReceivedJWTToken", new JwtToken(new JwtSecurityTokenHandler().WriteToken(jwtToken), jwtToken.ValidTo));
                        //return the token
                        return Ok(response);
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError,
                           new Response("Error", resultAdd.ToString()));
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
                    await _projectHelper.CreateUserInfor(newUser.Email);
                    var jwtToken = _projectHelper.GenerateJWTToken(newUser, await _userManager.GetRolesAsync(newUser));

                    _hubContext.Clients.All.SendAsync("ReceivedJWTToken", new JwtToken(new JwtSecurityTokenHandler().WriteToken(jwtToken), jwtToken.ValidTo));
                    //return Ok("done");
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                        expiration = jwtToken.ValidTo
                    });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                       new Response("Error", "Can not add login with external account"));
            }
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUser loginModel)
        {
            var loggedInUser = await _userManager.FindByEmailAsync(loginModel.Email);
            var passwordChecker = await _userManager.CheckPasswordAsync(loggedInUser, loginModel.Password);
            //checking if the user existed and password was valid
            if (loggedInUser == null || !passwordChecker) return Unauthorized(new Response("Error", "Invalid email or password"));
            if (!loggedInUser.IsActivated) return StatusCode(StatusCodes.Status405MethodNotAllowed, new Response("Error", "Inactive accounts are not allowed to log in"));

            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(loggedInUser, loginModel.Password, loginModel.RememberMe.Value, true);
            if (loggedInUser.TwoFactorEnabled)
            {
                var token = await _userManager.GenerateTwoFactorTokenAsync(loggedInUser, "Email");
                var message = new Message(new string[] { loggedInUser.Email! }, "OTP Confirmation", token);
                _emailService.SendEmail(message);
                return StatusCode(StatusCodes.Status200OK,
                 new Response("Success", $"The system has sent an OTP to your email {loggedInUser.Email}"));
            }
            var jwtToken = _projectHelper.GenerateJWTToken(loggedInUser, await _userManager.GetRolesAsync(loggedInUser));
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                expiration = jwtToken.ValidTo
            });
        }

        [HttpPost("login-2FA")]
        public async Task<IActionResult> LoginOTP(string twoFactorToken, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var signIn = await _signInManager.TwoFactorSignInAsync("Email", twoFactorToken, false, false);
            if (signIn.Succeeded)
            {
                if (user != null)
                {
                    var jwtToken = _projectHelper.GenerateJWTToken(user, await _userManager.GetRolesAsync(user));
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
        public async Task<IActionResult> ForgotPassword([Required] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                //generate a reset password token
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                //var forgotPasswordLink = Url.Action(nameof(ResetPassword), "Authentication", new { token, email = user.Email }, Request.Scheme);
                var forgotPasswordLink = $"http://localhost:8080/newpassword?email={email}&token={token}";
                var message = new Message(new string[] { user.Email }, "Reset password link:", $"Please click this url to reset your password:" + forgotPasswordLink);
                _emailService.SendEmail(message);
                return StatusCode(StatusCodes.Status200OK,
                   new Response("Success", $"Reset password link has sent to  {user.Email} successfully"));
            }
            return StatusCode(StatusCodes.Status400BadRequest,
                   new Response("Error", "This email is not linked to any account"));
        }


        [HttpPost("reset-password")]
        [AllowAnonymous]

        public async Task<IActionResult> SetNewPassword(ResetPassword resetPassword)
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user == null) return BadRequest(new Response("Error", "This email is not linked to any account"));
            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
            if (resetPassResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status200OK,
                       new Response("Success", resetPassword.Password));
            }
            else return BadRequest(new Response("Error", "Invalid token"));        
        }
    } 
}
