using GIL_Agent_Portal.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;

namespace GIL_Agent_Portal.Repositories
{
    public class ExternalApiRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ExternalApiRepository(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<SessionTokenResponse> GetSessionTokenAsync(SessionTokenRequest req)
        {
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://jiffyuat.nsdlbank.co.in/jarvisgwy/generatesessiontokenweb");
            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes("username:password")); // Basic Auth
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", auth);
            request.Content = JsonContent.Create(req);
            var response = await client.SendAsync(request);
            return await response.Content.ReadFromJsonAsync<SessionTokenResponse>();
        }
    }
}
