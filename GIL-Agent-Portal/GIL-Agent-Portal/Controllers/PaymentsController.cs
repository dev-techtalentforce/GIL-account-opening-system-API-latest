using GIL_Agent_Portal.Models;
using GIL_Agent_Portal.Repositories;
using GIL_Agent_Portal.Utlity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using static GIL_Agent_Portal.Utlity.RazorPayService;

namespace GIL_Agent_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly RazorPayService _service;



        public PaymentsController(RazorPayService service)
        {
            _service = service;

        }

        [HttpPost("paymentProcessing")]
        public IActionResult CreateOrder([FromBody] PaymentRequest request)
        {
            var orderId = _service.GenerateOrder(request);
            return Ok(new { orderId });
        }


        [HttpGet("GetByAgentId")]
        public async Task<IActionResult> GetByAgentId(string agentId)
        {
            var payments = await _service.GetPaymentsByAgentIdAsync(agentId);
            return Ok(payments);
        }

        [HttpPost("verifyPayment")]
        public IActionResult Verify([FromBody] RazorpayVerificationRequest req)
                {
            var isValid = _service.VerifyPayment(req);
            if (isValid)

                return Ok(new { status = "success" });
            return BadRequest(new { status = "failed" });
        }
    }
}
