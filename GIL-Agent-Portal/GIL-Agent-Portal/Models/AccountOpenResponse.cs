namespace GIL_Agent_Portal.Models
{
    public class AccountOpenResponse
    {
        public string AccountNumber { get; set; }
        public string CustomerId { get; set; }
        public string Cif { get; set; }
        public string CustomerName { get; set; }
        public string CustomerLastName { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string PartnerId { get; set; }
        public string ChannelId { get; set; }
        public string PartnerRefNumber { get; set; }
        public string CustomerRefNumber { get; set; }
        public string CustomerDematId { get; set; }
        public string CustomerClientId { get; set; }
        public string Response { get; set; }
        public string RespCode { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string Category { get; set; }
        public string Ifsccode { get; set; }
    }

    public class PartnerRedirectResult
    {
        public string FullUrl { get; set; }
    }
    public class PartnerCustomerModel
    {
        public string DecryptedData { get; set; }
        public string ResponseMessage { get; set; }
        public string ResponseCode { get; set; }
    }
}
