using System.Text.Json.Serialization;

namespace GIL_Agent_Portal.Models
{
    public class SessionTokenResponse
    {
        [JsonPropertyName("sessiontokendtls")]
        public SessionTokenDetails Sessiontokendtls { get; set; }
    }
    public class SessionTokenDetails
    {
        [JsonPropertyName("tokenkey")]
        public string TokenKey { get; set; }

        [JsonPropertyName("response")]
        public string Response { get; set; }

        [JsonPropertyName("expiredate")]
        public string ExpireDate { get; set; }

        [JsonPropertyName("respcode")]
        public string RespCode { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
