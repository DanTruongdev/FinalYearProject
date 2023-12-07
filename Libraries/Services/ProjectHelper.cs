﻿using Castle.Core.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OnlineShopping.Data;
using OnlineShopping.Libraries.Models;
using OnlineShopping.Models;
using OnlineShopping.Models.Customer;
using OnlineShopping.Models.Purchase;
using OnlineShopping.Models.Warehouse;
using OnlineShopping.ViewModels.VNPAY;
using OtpNet;
using RestSharp;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace OnlineShopping.Libraries.Services
{
    public class ProjectHelper : IProjectHelper
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;

        public ProjectHelper(ApplicationDbContext dbContext, UserManager<User> userManager, IConfiguration config)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _config = config;
        }

        public bool Refund(Order order)
        {
            var transition = GetTransition(order);
            if (transition == null) return false;
            var vnp_Api = _config["VNPAY:Api"];
            var vnp_HashSecret = _config["VNPAY:HashSecret"];
            var vnp_TmnCode = _config["VNPAY:TmnCode"];

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
            //string ipAddress = Response.HttpContext.Connection.RemoteIpAddress.ToString();
            //if (ipAddress.Equals("::1")) ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();

            string ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
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

            //string ipAddress = Response.HttpContext.Connection.RemoteIpAddress.ToString();
            //if (ipAddress.Equals("::1")) ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
            string ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
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
                TxnRef = (string)data["vnp_TxnRef"],
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

        public string UrlPayment(int typePayment, int orderId)
        {

            var order = _dbContext.Orders.FirstOrDefault(o => o.OrderId == orderId);

            //Get Config Info
            string vnp_Returnurl = _config["VNPAY:ReturnUrl"];
            string vnp_Url = _config["VNPAY:Url"];
            string vnp_TmnCode = _config["VNPAY:TmnCode"];
            string vnp_HashSecret = _config["VNPAY:HashSecret"];
            //Build URL for VNPAY
            VnPayService vnpay = new VnPayService();
            vnpay.AddRequestData("vnp_Version", VnPayService.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (order.TotalCost * 100000).ToString());
            var payment = _dbContext.Payments.Find(typePayment);
            vnpay.AddRequestData("vnp_BankCode", payment.PaymentMethod);
            vnpay.AddRequestData("vnp_CreateDate", order.OrderDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");

            string ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
            vnpay.AddRequestData("vnp_IpAddr", ipAddress);
            vnpay.AddRequestData("vnp_Locale", "en");
            vnpay.AddRequestData("vnp_OrderInfo", "Payment orders:" + order.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.OrderId.ToString());
            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return paymentUrl;
        }

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

        public JwtSecurityToken GenerateJWTToken(User user, IList<string> roles)
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

        public async Task<bool> CreateUserInfor(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;
            user.Point = 200;

            PointHistory newPointHistory = new PointHistory()
            {
                CustomerId = user.Id,
                Description = "Create account successfully +200 point",
                History = DateTime.Now
            };
            var announcement = new Announcement()
            {
                UserId = user.Id,
                Title = "Welcome",
                Content = "Welcome to L&L Store, wish you have a great shopping experience!",
                CreationDate = DateTime.Now
            };
            var cart = new Cart()
            {
                CustomerId = user.Id
            };
            var wishList = new WishList()
            {
                CustomerId = user.Id

            };
            try
            {
                _dbContext.Update(user);
                await _dbContext.AddRangeAsync(announcement, newPointHistory, cart, wishList);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }

        }


        public string CheckUserInfor(User user)
        {
            if (!user.EmailConfirmed) return "The user email must be confirmed";
            var defaultAddress = user.UserAddresses.IsNullOrEmpty() ? null : user.UserAddresses.FirstOrDefault(ua => ua.AddressType.Equals("DEFAULT"));
            if (user.UserAddresses.IsNullOrEmpty() || defaultAddress == null) return "Adding address is required to use this function";
            if (user.PhoneNumber == null || !user.PhoneNumberConfirmed) return "Adding and verifying phone number are required to use this function";
            return null;
        }



        public async Task<bool> CreateLogAsync(string assistantId, string activity)
        {
            var assistantExist = _dbContext.Users.FindAsync(assistantId);
            if (assistantExist == null) return false;
            Log newLog = new Log()
            {
                UserId = assistantId,
                Activity = activity,
                Date = DateTime.Now
            };
            try
            {
                await _dbContext.AddAsync(newLog);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }



        public string FilterBadWords(string input)
        {
            //filter vietnamese          
            string filePath = "VNbadwords.txt";
            string[] vnBadWords = System.IO.File.ReadAllLines(filePath);
            string pattern = @"\b(" + string.Join("|", vnBadWords.Select(Regex.Escape)) + @")\b";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            string content = regex.Replace(input, "***");

            //filter english
            var client = new RestClient("https://api.apilayer.com/bad_words?censor_character=***");
            var request = new RestRequest()
                .AddHeader("apikey", "ahpebO5MhcrobluOLxx0IbUwdpbd9viZ")
                .AddParameter("text/plain", content, ParameterType.RequestBody);
            var response = client.Post<object>(request);
            JObject data = JObject.Parse(response.ToString());
            return (string)data["censored_content"];
        }

        public async Task<bool> CreateAnnouncementAsync(User user, string title, string content)
        {
            Announcement newAnnouncement = new Announcement()
            {
                UserId = user.Id,
                Title = title,
                Content = content,
                CreationDate = DateTime.Now,
            };
            try
            {
                await _dbContext.AddAsync(newAnnouncement);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> UpdateCustomerSpentAsync(Order order)
        {
            try
            {
                var customer = order.Customer;
                customer.Spent += order.TotalCost;
                _dbContext.UpdateRange(customer);
                await _dbContext.SaveChangesAsync();
                return true;

            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CreatePointHistoryAsync(User user, int point, string description)
        {
            try
            {
                string subDesciption = point < 0 ? $"{point}" : $"+{point}";
                var newPointHistory = new PointHistory()
                {
                    CustomerId = user.Id,
                    Description = description + " " + subDesciption,
                    History = DateTime.Now
                };
                await _dbContext.AddAsync(newPointHistory);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
