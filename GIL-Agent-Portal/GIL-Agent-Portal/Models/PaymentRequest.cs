namespace GIL_Agent_Portal.Models
{
    public class PaymentRequest
    {
        public string AgentID { get; set; }
        public int Amount { get; set; }
    }

    public class RZPCheckoutPayment
    {
        public int amount { get; set; }
        public string receipt { get; set; }
        public string orderID { get; set; }
        public string AgentId { get; set; }
        public string paymentStatus { get; set; }
        public string RequestData { get; set; }
        public string ResponseData { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
