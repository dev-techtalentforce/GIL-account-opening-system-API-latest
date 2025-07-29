namespace GIL_Agent_Portal.Models
{
    public class PaymentHistoryResponse
    {
       
            //public int PhId { get; set; }
            //public int PhAgentId { get; set; }
            public string PhOrderId { get; set; } = string.Empty;
            public int PhPaidAmount { get; set; }
            public int PhStatus { get; set; }
            public int? PhCreditId { get; set; }
            public int? PhCreditValue { get; set; }
        

    }
}
