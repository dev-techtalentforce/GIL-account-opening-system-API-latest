namespace GIL_Agent_Portal.Models
{
    public class SMSRequest
    {
        public string channelid { get; set; }
        public string appid { get; set; }
        public string custunqid { get; set; }
        public string mobileno { get; set; }
        public string msg_text { get; set; }
        public string msg_type { get; set; } = "TXNALERT";
        public string referenceno { get; set; }
        public string token { get; set; }
        public string signcs { get; set; }
    }
}
