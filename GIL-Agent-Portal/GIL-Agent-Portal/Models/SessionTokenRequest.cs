namespace GIL_Agent_Portal.Models
{
    public class SessionTokenRequest
    {
        public string channelid { get; set; }
        public string appid { get; set; }
        public string userid { get; set; }
        public string usertype { get; set; } = "PARTNER";
        public string tokenkey { get; set; } // Combine Token + Key, if needed
    }
}
