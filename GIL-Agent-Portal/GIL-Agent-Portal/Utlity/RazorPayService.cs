using Dapper;
using GIL_Agent_Portal.Models;
using GIL_Agent_Portal.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Razorpay.Api;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace GIL_Agent_Portal.Utlity
{
    public class RazorPayService
    {
        private readonly RazorPayRepository _repository;
        private static string YOUR_KEY_ID = "rzp_test_0Uh8LaT1c4HiZc";
        private static string YOUR_KEY_SECRET = "jEnPDu2nt1zl1nLcGQc6iYdU";
        private readonly RazorpayClient _client;
        private readonly string _connectionString;

        public RazorPayService(RazorPayRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _client = new RazorpayClient(YOUR_KEY_ID, YOUR_KEY_SECRET);
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        public class OrderDetails
        {
            public string OrderID { get; set; }
            public int Amount { get; set; }
            public string Receipt { get; set; }
            public string Status { get; set; }
            public string RequestData { get; set; }
            public string ResponseData { get; set; }
        }

        public string GenerateOrder(int amount,string agentId)
        {
            try
            {
                RazorpayClient client = new RazorpayClient(YOUR_KEY_ID, YOUR_KEY_SECRET);

                string receiptId = "rcpt_" + Guid.NewGuid().ToString().Substring(0, 8);
                var orderRequest = new Dictionary<string, object>
            {
                { "amount", amount * 100 },  // in paise
                { "currency", "INR" },
                { "receipt", receiptId },
                { "notes", new Dictionary<string, object>
                    {
                        { "BusinessID", "AutoGen" },
                        { "PlanID", "AutoGen" }
                    }
                }
            };

                string requestJson = JsonConvert.SerializeObject(orderRequest);
                Order order = client.Order.Create(orderRequest);
                string responseJson = JsonConvert.SerializeObject(order);
                                
                var paymentModel = new RZPCheckoutPayment
                {
                    amount = amount,
                    receipt = receiptId,
                    AgentId = agentId,
                    orderID = order["id"].ToString(),
                    paymentStatus = "Created",
                    RequestData = requestJson,
                    ResponseData = responseJson,
                   
                };

                LogPaymentToDatabase(paymentModel);

                return order["id"];
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private bool LogPaymentToDatabase(RZPCheckoutPayment data)
        {
            var sp = "SaveRazorpayPayment";
            var parameters = new DynamicParameters();

            parameters.Add("@OrderID", data.orderID.ToString());
            parameters.Add("@Amount", data.amount);
            parameters.Add("@Receipt", data.receipt);
            parameters.Add("@AgentId", (object)data.AgentId ?? DBNull.Value);
            parameters.Add("@Status", data.paymentStatus);
            parameters.Add("@RequestData", data.RequestData);
            parameters.Add("@ResponseData", data.ResponseData);

            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    db.Open();

                    // Execute the stored procedure to save payment data
                    var result = db.ExecuteScalar<int>(sp, parameters, commandType: CommandType.StoredProcedure);

                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                // Log the exception (you can implement a logger here if needed)
                // e.g., _logger.LogError(ex, "Error in saving Razorpay payment data");
                return false;
            }
        }
        public class RazorpayVerificationRequest
        {
            public string OrderId { get; set; }
            public string? PaymentId { get; set; }
            public string? Signature { get; set; }
        }

        public bool VerifyPayment(RazorpayVerificationRequest req)
        {
            RazorpayClient client = new RazorpayClient(YOUR_KEY_ID, YOUR_KEY_SECRET);

            Order order = client.Order.Fetch(req.OrderId);

            string orderId = order["id"].ToString();
            string status = order["status"].ToString();
            int amount = Convert.ToInt32(order["amount"]);

            return true;
            
        }


        public async Task<IEnumerable<OrderDetails>> GetPaymentsByAgentIdAsync(string agentId)
        {
            var sp = "GetRazorpayPaymentsByAgentId";
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                try
                {
                    var parameters = new { AgentId = agentId };

                    var result = await db.QueryAsync<OrderDetails>(
                        sp,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error retrieving Razorpay payments: {ex.Message}", ex);
                }
            }
        }

    }
}
