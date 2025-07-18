using GIL_Agent_Portal.Models;
using GIL_Agent_Portal.Utlity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;
using System.Security.Cryptography;
using System.Text;

namespace GIL_Agent_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public PaymentsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }     

        [HttpPost("verify")]
        public IActionResult VerifyPayment([FromBody] RZPCheckoutPayment request)
        {
           
            var result = RazorPayService.GenerateOrder(request);
            return Ok(result);  
           
        }

        //private string GetRazorpaySignature(string data, string secret)
        //{
        //    var keyBytes = Encoding.UTF8.GetBytes(secret);
        //    var messageBytes = Encoding.UTF8.GetBytes(data);

        //    using var hmac = new HMACSHA256(keyBytes);
        //    var hash = hmac.ComputeHash(messageBytes);
        //    return BitConverter.ToString(hash).Replace("-", "").ToLower();
        //}
    }
}
