using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShopping.ViewModels.SignUp;
using OnlineShopping.ViewModels;
using OnlineShopping.Libraries.Services;
using NETCore.MailKit.Core;
using OnlineShopping.Libraries.Models;
using OnlineShopping.Models;

namespace OnlineShopping.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly EmailSevice _emailService;
        public AuthenticationController(UserManager<User> userManager,
            RoleManager<IdentityRole> userRole, SignInManager<User> signInManager, IConfiguration config, EmailSevice emailService)
        {
            _userManager = userManager;
            _roleManager = userRole;
            _signInManager = signInManager;
            _config = config;
            _emailService = emailService;
        }

    //    [HttpPost]
    //    [Route("register")]
    //    public async Task<IActionResult> Register([FromBody] RegisterUser resgisterUser, string role)
    //    {
            
    //        //ensure user has not  already created!
    //        var userExist = await _userManager.FindByEmailAsync(resgisterUser.Email);
    //        if (userExist != null)
    //        {
    //            return StatusCode(StatusCodes.Status406NotAcceptable,
    //                new Response { Status = "Error", Message = "The user already exist!" });
    //        }
    //        //ensure database contains the role
    //        if (!await _roleManager.RoleExistsAsync("CUSTOMER"))//test
    //        {
    //            return StatusCode(StatusCodes.Status500InternalServerError,
    //                    new Response { Status = "Error", Message = "The role does not exist" });
    //        }
    //        //User newUser = new User()
    //        //{
    //        //    FirstName = resgisterUser.FirstName,
    //        //    LastName = resgisterUser.LastName,
    //        //    DoB = resgisterUser.DoB,
    //        //    Gender = resgisterUser.Gender,
    //        //    UserName = resgisterUser.Username,
    //        //    PhoneNumber = "0123456789",
    //        //    Email = resgisterUser.Email,
    //        //    CreationDate = DateTime.Now,
    //        //    TwoFactorEnabled = false
    //        //};
    //        User newUser = new User()
    //        {
    //            FirstName = "Test",
    //            LastName = "Test",
    //            DoB = DateTime.Now,
    //            Gender = "Male",
    //            UserName = "Tester123",
    //            PhoneNumber = "1234567890",
    //            PhoneNumberConfirmed = false,
    //            Email = "Testter1234@gmail.com",
    //            CreationDate = DateTime.Now,
    //            TwoFactorEnabled = false
    //        };
    //        var result = await _userManager.CreateAsync(newUser, "DSS123");//test          
    //        if (!result.Succeeded)
    //        {
    //            return StatusCode(StatusCodes.Status500InternalServerError,
    //                new Response { Status = "Error", Message = result.ToString()});
    //        }
    //        await _userManager.AddToRoleAsync(newUser, "CUSTOMER");

    //        var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
    //        var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authentication", new { token, email = newUser.Email }, Request.Scheme);
    //        var message = new Message(new string[] { newUser.Email! }, "Confirmation email link", confirmationLink!);
    //        _emailService.SendEmail(message);
    //        return StatusCode(StatusCodes.Status200OK,
    //            new Response { Status = "Success", Message = $"User created & Email Sent to {newUser.Email} SuccessFully" });
            
            
    //    }
    }
}
