using GIL_Agent_Portal.Models;
using System.Text.Json;
using System.Text;
using GIL_Agent_Portal.Repositories.Interface;

namespace GIL_Agent_Portal.Repositories
{
    public class BcAgentRegistrationRepository : IBcAgentRegistrationRepository
    {
        public async Task<BcAgentRegistrationResponse> SubmitAgentRegistrationAsync(BcAgentRegistrationRequest model)
        {
            using var client = new HttpClient();
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://jiffyuat.nsdlbank.co.in/jarvisgwy/partner/bcagentregistration", content);
            response.EnsureSuccessStatusCode();

            var raw = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BcAgentRegistrationResponse>(raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }
    }
}
