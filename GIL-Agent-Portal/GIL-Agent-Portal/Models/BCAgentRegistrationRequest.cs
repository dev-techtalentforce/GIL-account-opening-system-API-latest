namespace GIL_Agent_Portal.Models
{
    public class BcAgentRegistrationRequest
    {
        public string channelid { get; set; }
        public string appid { get; set; }
        public string partnerid { get; set; }
        public string bcid { get; set; }
        public string bcagentid { get; set; }
        public string bcagentname { get; set; }
        public string middlename { get; set; }
        public string lastname { get; set; }
        public string companyname { get; set; }
        public string address { get; set; }
        public string statename { get; set; }
        public string cityname { get; set; }
        public string district { get; set; }
        public string area { get; set; }
        public string pincode { get; set; }
        public string mobilenumber { get; set; }
        public string telephone { get; set; }
        public string alternatenumber { get; set; }
        public string emailid { get; set; }
        public string dob { get; set; }
        public string shopaddress { get; set; }
        public string shopstate { get; set; }
        public string shopcity { get; set; }
        public string shopdistrict { get; set; }
        public string shoparea { get; set; }
        public string shoppincode { get; set; }
        public string pancard { get; set; }
        public string bcagentform { get; set; }
        public ProductDetails productdetails { get; set; }
        public TerminalDetails terminaldetails { get; set; }
        public string agenttype { get; set; }
        public string agentbcid { get; set; }
        public string token { get; set; }
        public string signcs { get; set; }
    }

    public class ProductDetails
    {
        public string dmt { get; set; }
        public string aeps { get; set; }
        public string cardpin { get; set; }
        public string accountopen { get; set; }
    }

    public class TerminalDetails
    {
        public string tposserialno { get; set; }
        public string taddress { get; set; }
        public string taddress1 { get; set; }
        public string tpincode { get; set; }
        public string tcity { get; set; }
        public string tstate { get; set; }
        public string temail { get; set; }
    }

}
