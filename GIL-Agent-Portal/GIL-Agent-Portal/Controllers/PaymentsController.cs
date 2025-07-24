using GIL_Agent_Portal.Models;
using GIL_Agent_Portal.Repositories;
using GIL_Agent_Portal.Utlity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace GIL_Agent_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly RazorPayService _service;


        public PaymentsController(IDbConnection dbConnection, ILogger<RazorPayRepository> logger)
        {
            var repo = new RazorPayRepository(dbConnection, logger);
            _service = new RazorPayService(repo);
        }

        [HttpPost("verify")]
        public IActionResult CreateOrder([FromBody] PaymentRequest request)
        {
            var orderId = _service.GenerateOrder(request.Amount);
            return Ok(new { orderId });
        }
    }
}
