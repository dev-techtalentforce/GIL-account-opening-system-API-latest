namespace GIL_Agent_Portal.Models
{
    public class BcAgentRegistrationResponse
    {
        public string response { get; set; }
        public BcAgentData AgentData { get; set; }
        public string respcode { get; set; }
    }

    public class BcAgentData
    {
        public BcAgentRegistrationResult bcagentregistrationres { get; set; }
    }

    public class BcAgentRegistrationResult
    {
        public string bcagentid { get; set; }
        public string description { get; set; }
        public int status { get; set; }
    }
}