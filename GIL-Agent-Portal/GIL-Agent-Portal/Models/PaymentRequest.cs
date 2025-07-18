namespace GIL_Agent_Portal.Models
{
    public class PaymentRequest
    {
        public string AgentId { get; set; }
        public int Amount { get; set; }
    }

    public class RZPCheckoutPayment
    {
        public decimal amount { get; set; }
        public int businessID { get; set; }
        public int planID { get; set; }
        public string receipt { get; set; }
        public string orderID { get; set; }
        public string transID { get; set; }
        public string paymentStatus { get; set; }
    }
}
