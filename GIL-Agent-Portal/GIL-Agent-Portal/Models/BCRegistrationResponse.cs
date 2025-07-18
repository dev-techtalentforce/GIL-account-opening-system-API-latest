namespace GIL_Agent_Portal.Models
{
    public class BcRegistrationResponse
    {
        public string response { get; set; }
        public AgentData AgentData { get; set; }
        public string respcode { get; set; }
    }

    public class AgentData
    {
        public BcRegistrationNewRes bcregistrationnewres { get; set; }
    }

    public class BcRegistrationNewRes
    {
        public int bcid { get; set; }
        public string password { get; set; }
        public string username { get; set; }
    }
}
