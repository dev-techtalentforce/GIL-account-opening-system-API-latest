using GIL_Agent_Portal.Models;
using GIL_Agent_Portal.Repositories;
using Newtonsoft.Json;
using Razorpay.Api;
using System.Data;

namespace GIL_Agent_Portal.Utlity
{
    public class RazorPayService
    {
        private readonly RazorPayRepository _repository;
        private static string YOUR_KEY_ID = "rzp_test_0Uh8LaT1c4HiZc";
        private static string YOUR_KEY_SECRET = "jEnPDu2nt1zl1nLcGQc6iYdU";

        public RazorPayService(RazorPayRepository repository)
        {
            _repository = repository;
        }

       
    public string GenerateOrder(int amount)
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
                    businessID = 0, // Or use data from notes if available
                    planID = 0,
                    receipt = receiptId,
                    orderID = order["id"].ToString(),
                    transID = "", // To be updated later
                    paymentStatus = "Created",
                    RequestData = requestJson,
                    ResponseData = responseJson
                };

                _repository.SavePaymentToDB(paymentModel, paymentModel.orderID, requestJson, responseJson);

                return paymentModel.orderID;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }

}
