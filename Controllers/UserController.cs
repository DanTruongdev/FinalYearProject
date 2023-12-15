using Castle.Core.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopping.Data;
using OnlineShopping.Libraries.Models;
using OnlineShopping.Libraries.Services;
using OnlineShopping.Models;
using OnlineShopping.Models.Customer;
using OnlineShopping.ViewModels;
using OnlineShopping.ViewModels.Address;
using OnlineShopping.ViewModels.User;
using OtpNet;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;

namespace OnlineShopping.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IFirebaseService _firebaseService;
        private readonly ISMSService _smsService;
        private readonly IProjectHelper _projectHelper;
        private readonly IEmailService _emailService;


        public UserController(ApplicationDbContext dbContext, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IFirebaseService firebaseService, ISMSService smsService,
            IProjectHelper projectHelper, IEmailService emailService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _firebaseService = firebaseService;
            _smsService = smsService;
            _projectHelper = projectHelper;
            _emailService = emailService;
        }

        //ADD account in authentication controller
        //Search
        [HttpGet("search")]
        public async Task<IActionResult> SearchUser(string searchString)
        {
            var users = await _userManager.Users.Where(u => u.FirstName.Contains(searchString) || u.LastName.Contains(searchString) || u.Email.Contains(searchString)).ToListAsync();
            if (users.IsNullOrEmpty()) return Ok(new List<User>());
            var response = users.Select(u => new
            {
                UserId = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                DoB = u.DoB,
                Gender = u.Gender,
                UserName = u.Email,
                Role = _userManager.GetRolesAsync(u).Result.FirstOrDefault(),
                PhoneNumber = u.PhoneNumber,
                PhoneNumberConfirmed = u.PhoneNumberConfirmed,
                Email = u.Email,
                EmailConfirmed = u.EmailConfirmed,
                Avatar = _firebaseService.GetDownloadUrl(u.Avatar),
                CreationDate = u.CreationDate,
                LatestUpdate = u.LatestUpdate,
                IsActivated = u.IsActivated,
                TwoFactorEnabled = u.IsActivated,
                Debit = u.Debit,
                Spent = u.Spent,
                Point = u.Point

            });
            return Ok(response);
        }

        //View all {shop-owner}
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUser([FromQuery] string? roleId)
        {
            string userRole = "ALL";
            if (!roleId.IsNullOrEmpty())
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null) return NotFound(new Response("Error", $"The role with id = {roleId} was not found"));
                userRole = role.Name;
            }
            var data = userRole.Equals("ALL") ? await _userManager.Users.ToListAsync() : await _userManager.GetUsersInRoleAsync(userRole);
            var response = data.Select(d => new
            {
                UserId = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                DoB = d.DoB,
                Gender = d.Gender,
                UserName = d.Email,
                Role = _userManager.GetRolesAsync(d).Result.FirstOrDefault(),
                PhoneNumber = d.PhoneNumber,
                Email = d.Email,
                Avatar = _firebaseService.GetDownloadUrl(d.Avatar),
                CreationDate = d.CreationDate,
                IsActivated = d.IsActivated,
                TwoFactorEnabled = d.IsActivated,
                Debit = d.Debit,
                Spent = d.Spent,
                Point = d.Point
            });
            return Ok(response);
        }

        //detail {all}
        [HttpGet("detail")]
        public async Task<IActionResult> GetUserDetail()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return Unauthorized(new Response("Error", "Logged in user not found "));
            var loggedInUser = await _userManager.FindByEmailAsync(email);
            var response = new
            {
                UserId = loggedInUser.Id,
                FirstName = loggedInUser.FirstName,
                LastName = loggedInUser.LastName,
                DoB = loggedInUser.DoB,
                Gender = loggedInUser.Gender,
                UserName = loggedInUser.Email,
                Role = _userManager.GetRolesAsync(loggedInUser).Result.FirstOrDefault(),
                PhoneNumber = loggedInUser.PhoneNumber,
                PhoneNumberConfirmed = loggedInUser.PhoneNumberConfirmed,
                Email = loggedInUser.Email,
                EmailConfirmed = loggedInUser.EmailConfirmed,
                Avatar = _firebaseService.GetDownloadUrl(loggedInUser.Avatar),
                CreationDate = loggedInUser.CreationDate,
                LatestUpdate = loggedInUser.LatestUpdate,
                IsActivated = loggedInUser.IsActivated,
                TwoFactorEnabled = loggedInUser.IsActivated,
                Debit = loggedInUser.Debit,
                Spent = loggedInUser.Spent,
                Point = loggedInUser.Point
            };
            return Ok(response);
        }

        //update one {all}
        [HttpPut("individual/update")]
        public async Task<IActionResult> UpdateUserInfor([FromForm] EditInforViewModel userInput)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found ");
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            loggedInUser.FirstName = userInput.FirstName == null ? loggedInUser.FirstName : userInput.FirstName;
            loggedInUser.LastName = userInput.LastName == null ? loggedInUser.LastName : userInput.LastName; ;
            loggedInUser.DoB = userInput.DoB == null ? loggedInUser.DoB : userInput.DoB; ;
            loggedInUser.Gender = userInput.Gender == null ? loggedInUser.Gender : userInput.Gender;
            loggedInUser.LatestUpdate = DateTime.Now;

            if (userInput.Image != null)
            {
                bool isRemoved = true;
                if (loggedInUser.Avatar != null) isRemoved = _firebaseService.RemoveFile(loggedInUser.Avatar);
                if (!isRemoved) return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", "An error occurs when  uploading file"));
                loggedInUser.Avatar = _firebaseService.UploadFile(userInput.Image);
            }
            try
            {
                _dbContext.Update(loggedInUser);
                await _dbContext.SaveChangesAsync();
                return Ok("Update user information successfully");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   new Response("Error", "An error occurs when updating user information"));
            }

        }


        //Update all {shop owner}
        [HttpPut("all/update")]
        public async Task<IActionResult> UpdateAllUserInfor([FromForm] EditAllInforViewModel userInput)
        {
            User userExist = await _userManager.FindByIdAsync(userInput.UserId);
            if (userExist == null) return NotFound(new Response("Error", $"The user with id = {userInput.UserId} was not found"));
            userExist.FirstName = userInput.FirstName == null ? userExist.FirstName : userInput.FirstName;
            userExist.LastName = userInput.LastName == null ? userExist.LastName : userInput.LastName; ;
            userExist.DoB = userInput.DoB == null ? userExist.DoB : userInput.DoB; ;
            userExist.Gender = userInput.Gender == null ? userExist.Gender : userInput.Gender;
            userExist.LatestUpdate = DateTime.Now;

            if (userInput.Image != null)
            {
                bool isRemoved = true;
                if (userExist.Avatar != null) isRemoved = _firebaseService.RemoveFile(userExist.Avatar);
                if (!isRemoved) return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", "An error occurs when  uploading file"));
                userExist.Avatar = _firebaseService.UploadFile(userInput.Image);
            }
            try
            {
                _dbContext.Update(userExist);
                await _dbContext.SaveChangesAsync();
                return Ok("Update user information successfully");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   new Response("Error", "An error occurs when updating user information"));
            }

        }

        //2FA
        //2fa {all}
        [HttpPut("individual/toggle-2fa")]

        public async Task<IActionResult> ToggleUserTwoFactor()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found ");
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            try
            {
                loggedInUser.TwoFactorEnabled = !loggedInUser.TwoFactorEnabled;
                _dbContext.Update(loggedInUser);
                await _dbContext.SaveChangesAsync();
                return Ok("Successfully");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response("Error", "An error occurs when processing 2fa"));
            }

        }

        //2fa {shop owner}
        [HttpPut("all/toggle-2fa")]

        public async Task<IActionResult> ToggleAllUserTwoFactor([Required] string userId)
        {
            User userExist = await _userManager.FindByIdAsync(userId);
            if (userExist == null) return BadRequest(new Response("Error", $"The user with id = {userId} was not found"));
            try
            {
                userExist.TwoFactorEnabled = !userExist.TwoFactorEnabled;
                _dbContext.Update(userExist);
                await _dbContext.SaveChangesAsync();
                return Ok("Successfully");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response("Error", "An error occurs when processing 2fa"));
            }

        }

        //PHONE NUMBER - shop owner not allows edit phone number of other role

        [HttpGet("individual/phone-number/get-otp")]
        public async Task<IActionResult> SendOTPConfirmation([Required] string phoneNums)
        {
            var phoneExist = await _dbContext.Users.FirstOrDefaultAsync(u => u.PhoneNumber.Equals(phoneNums) && u.PhoneNumberConfirmed == true);
            if (phoneExist != null) return StatusCode(StatusCodes.Status406NotAcceptable,
                new Response("Error", "This phone number is already linked to another account"));

            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound(new Response("Error", "Logged in user not found "));
            var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));

            byte[] secretKey = Encoding.ASCII.GetBytes(phoneNums);
            var otp = new Totp(secretKey, step: 300, totpSize: 6);
            var totpCode = otp.ComputeTotp(DateTime.Now);
            var sms = new Sms(new string[] { phoneNums }, $"The OTP to confirm your phone number: {totpCode}");
            _smsService.SendSMS(sms);
            return Ok($"The confirmation has sent to {phoneNums} successfully");
        }

        //ADD
        [HttpPost("individual/phone-number/add")]
        public async Task<IActionResult> AddPhoneNumber(string phoneNumber, string otp)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged user not found ");
            var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
           
            var result = await _projectHelper.VerifyPhoneNum(phoneNumber, otp);
            if (!result) return StatusCode(StatusCodes.Status406NotAcceptable,
                new Response("Error", "The OTP is not correct"));
            loggedInUser.PhoneNumber = phoneNumber;
            loggedInUser.PhoneNumberConfirmed = true;
            _dbContext.Update(loggedInUser);
            await _dbContext.SaveChangesAsync();
            return Accepted("Add phone number successfully");
        }

        //UPDATE
        [HttpPut("individual/phone-number/update")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> ChangeCustomerPhoneNum(string phoneNumber, string otp)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged user not found ");
            var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            var result = await _projectHelper.VerifyPhoneNum(phoneNumber, otp);
            if (!result)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable,
                    new Response("Error", "The OTP is incorrect"));
            }
            loggedInUser.PhoneNumber = phoneNumber;
            loggedInUser.PhoneNumberConfirmed = true;
            _dbContext.Update(loggedInUser);
            await _dbContext.SaveChangesAsync();
            return Accepted("Change phone number successfully");
        }

        //ACTIVATION
        //disable individual
        [HttpPut("individual/disable-account")]
        public async Task<IActionResult> DisableUserAccount([Required] string password)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound(new Response("Error", "Logged in user not found "));
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            var passChecker = await _userManager.CheckPasswordAsync(loggedInUser, password);
            if (!passChecker) return Unauthorized(new Response("Error", "Incorrect password"));
            try
            {
                loggedInUser.IsActivated = false;
                _dbContext.Update(loggedInUser);
                await _dbContext.SaveChangesAsync();
                return Ok(new Response("Success", "Disable account successfully"));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response("Error", "An error occurs when disable account"));
            }
        }

        //enable individual
        [HttpGet("individual/enable-account-otp")]
        public async Task<IActionResult> GetEnableUserAccountOTP([Required] string email)
        {
            User userExist = await _userManager.FindByEmailAsync(email);
            if (userExist == null) return NotFound(new Response("Error", $"{email} is not linked to any account"));

            byte[] secretKey = Encoding.ASCII.GetBytes(email);
            var otp = new Totp(secretKey, step: 300, totpSize: 6);
            var totpCode = otp.ComputeTotp(DateTime.Now);

            var message = new Message(new string[] { userExist.Email! }, $"The OTP is to enable your account: ", totpCode);
            _emailService.SendEmail(message);
            return Ok(new Response("Success", $"The enable account OTP has been sent to {email}"));
        }

        [HttpPut("individual/enable-account")]
        public async Task<IActionResult> EnableUserAccount([Required] string email, [Required] string totpCode)
        {
            User userExist = await _userManager.FindByEmailAsync(email);
            if (userExist == null) return NotFound(new Response("Error", $"{email} is not linked to any account"));

            var result = await _projectHelper.VerifyPhoneNum(email, totpCode);
            if (!result) return StatusCode(StatusCodes.Status406NotAcceptable,
                new Response("Error", "The OTP is not correct"));
            try
            {
                userExist.IsActivated = true;
                _dbContext.Update(userExist);
                await _dbContext.SaveChangesAsync();
                return Ok(new Response("Success", "Enable account successfully"));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response("Error", "An error occurs when enable account"));
            }
        }

        //{shop-owner}
        [HttpPut("all/toggle-account-activation")]
        public async Task<IActionResult> ToggleAccountActivation([Required] string userId, [Required] string password)
        {
            User userExist = await _userManager.FindByIdAsync(userId);
            if (userExist == null) return NotFound(new Response("Error", $"The user with id = {userId} was not found"));
            var pwdChecker = await _userManager.CheckPasswordAsync(userExist, password);
            if (!pwdChecker) return BadRequest(new Response("Error", "The password is incorrect"));
            try
            {
                userExist.IsActivated = !userExist.IsActivated;
                _dbContext.Update(userExist);
                await _dbContext.SaveChangesAsync();
                return Ok(new Response("Success", $"Activation status = {userExist.IsActivated}"));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response("Error", "An error occurs when change activation status"));
            }
        }

                                                                                            //ADDRESS
        //VIEW
        [HttpGet("customer-infor/address")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetCustomerAddress()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found ");
            var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            var addressList = loggedInUser.UserAddresses.Select(a => new
            {
                Id = a.Address.AddressId,
                Street = a.Address.Street,
                Ward = a.Address.Ward,
                District = a.Address.District,
                Provine = a.Address.Provine,
                AddressType = a.AddressType
            });
            if (addressList.Count() == 0) return NotFound("User has no address added");
            return Ok(addressList);
        }

        //CREATE
        [HttpPost("customer-infor/address/add")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> AddCustomerAddress([FromForm] AddCustomerAddressViewModel userInput)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found ");
            var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            if (userInput.Type.Equals("DEFAULT"))
            {
                var defaultAddress = loggedInUser.UserAddresses.FirstOrDefault(ua => ua.AddressType.Equals("DEFAULT"));
                if (defaultAddress != null) return StatusCode(StatusCodes.Status406NotAcceptable,
                    new Response("Error", "Cannot set more than one default address"));
            }
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
                UserId = loggedInUser.Id,
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
            var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
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
        public async Task<IActionResult> RemoveUserAddress([Required] int id)
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
                    new Response("Error", "The address remove failed"));
            }

        }


        [HttpGet("customer-infor/csv")]
        public async Task<IActionResult> DownloadAllUserInforCSV()
        {
            var data = await _dbContext.Users.ToListAsync();
            var csv = new StringBuilder();
            string latestUpdate = "Latest Update: " + DateTime.Now.ToString();
            string heading = "UserId,First Name,Last Name,Email,Phone Number,Role,Default Address,Gender,Spent,Debit,Creation Date, Status";
            csv.AppendLine(latestUpdate);
            csv.AppendLine(heading);
            foreach (var user in data)
            {
                var role = _userManager.GetRolesAsync(user).Result.First();
                var userAddress = user.UserAddresses.FirstOrDefault(a => a.AddressType.Equals("DEFAULT"));
                var address = userAddress == null ? "" : userAddress.Address.ToString();
                address = $"\"{address}\"";
                var newRow = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                         user.Id,
                         user.FirstName,
                         user.LastName,
                         user.Email,
                         user.PhoneNumber,
                         role,
                         address,
                         user.Gender,
                         user.Spent,
                         user.Debit,
                         user.CreationDate.ToString(),
                         user.IsActivated ? "Activated" : "Inactivated"
                       );
                csv.AppendLine(newRow);
            }
            byte[] bytes = Encoding.ASCII.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "User_Information.csv");
        }
    }
}
