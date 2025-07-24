using GIL_Agent_Portal.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text;

namespace GIL_Agent_Portal.Services
{
    public class SessionTokenService
    {
        private const string SessionTokenApiUrl = "https://jiffyuat.nsdlbank.co.in/jarvisgwy/generatesessiontokenweb";
        private const string Username = "wpemmjhKus";
        private const string Password = "\\c|jF1764E6d";

        public async Task<SessionTokenResponse> FetchNsdlSessionTokenAsync()
        {
            try
            {
                var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Username}:{Password}"));

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var requestPayload = new
                {
                    channelid = "lfbpWjegXHwnnirQOlYP",
                    appid = "com.jarviswebbc.nsdlpb",
                    userid = "919510352639",
                    usertype = "PARTNER",
                    partnerid = Username,
                    token = "NA"
                };

                var json = JsonSerializer.Serialize(requestPayload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(SessionTokenApiUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"HTTP Error: {response.StatusCode}");
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                    return null;
                }

                var rawResponse = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<SessionTokenResponse>(rawResponse, options);
                return result;
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Request error: {httpEx.Message}");
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"JSON error: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }

            return null;
        }
    }
}
