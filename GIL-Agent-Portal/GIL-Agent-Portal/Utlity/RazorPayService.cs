using GIL_Agent_Portal.Models;
using Razorpay.Api;

namespace GIL_Agent_Portal.Utlity
{
    public class RazorPayService
    {
        /* Dev */
        // private static final String YOUR_KEY_ID = "rzp_test_wta0cZwWHRH7Yc";
        // private static final String YOUR_KEY_SECRET = "bIdmdBXsBMCeutS63BK7QG1a";
        /* Live */
        private static string YOUR_KEY_ID = "rzp_test_0Uh8LaT1c4HiZc";
        private static string YOUR_KEY_SECRET = "jEnPDu2nt1zl1nLcGQc6iYdU";
        public static string GenerateOrder(RZPCheckoutPayment req)
        {
            try
            {
                RazorpayClient razorpay = new RazorpayClient(YOUR_KEY_ID, YOUR_KEY_SECRET);
                Dictionary<string, object> orderRequest = new Dictionary<string, object>
             {
                 { "amount", req.amount * 100 },
                 { "currency", "INR" },
                 { "receipt", req.receipt }
             };
                Dictionary<string, object> notes = new Dictionary<string, object>();
                notes.Add("BusinessID", req.businessID);
                notes.Add("PlanID", req.planID);
                orderRequest.Add("notes", notes);

                Order order = razorpay.Order.Create(orderRequest);


                return order["id"];
            }
            catch (Exception e)
            {
                //e.printStackTrace();
            } 

            return "";
        }
    }
}
