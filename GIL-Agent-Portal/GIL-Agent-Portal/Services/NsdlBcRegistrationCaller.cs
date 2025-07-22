using GIL_Agent_Portal.Models;
using GIL_Agent_Portal.Utlity;
using System.Text.Json;
using System.Text;

namespace GIL_Agent_Portal.Services
{
    public class NsdlBcRegistrationCaller
    {
        public async Task<string> CallBcRegistrationAsync()
        {
            var session = await new SessionTokenService().FetchNsdlSessionTokenAsync();
            string tokenKey = session.Sessiontokendtls.TokenKey;
            string token = NsdlSignCsHelper.ExtractToken(tokenKey);
            string key = NsdlSignCsHelper.ExtractKey(tokenKey);

            var request = new BcRegistrationRequest
            {
                appid = "com.jarviswebbc.nsdlpb",
                partnerid = "wpemmjhKus",
                ceoname = "JAKIR ANSARI",
                mobile = "8140528022",
                telphonenumber = "",
                emailid = "sbhojpuri@gmail4.com",
                pancard = "NLXBC1905E",
                companyname = "metizsoftQAS",
                address = "WARD06VILLAGEBHAGWANPURTOLAPOSTYOGIVANABAJARANCHALBATNAHABHAGWANPURURFPATAHISITAMARHIBIHAR843322",
                pincode = "843322",
                city = "SITAMARHI",
                state = "Bihar",
                district = "SITAMARHI",
                dateofagreement = "08/21/2024",
                channelid = "lfbpWjegXHwnnirQOlYP",
                channelkey = "vvYSTiZULlthKgCzAtksvjVvdKrrvDTBaDUWZkEjqCbHeOncrjGrqLeYFPKOlsWmpODkRHjqwLAZyIYUuxjlOxafSekLcaVfQesvWWTnvGRoUCtANtAApwRIsovthEWN",
                custunqid = "wpemmjhKus",
                callbackurl = "AG01023",
                dmt = 1,
                aeps = 1,
                cardpin = 0,
                accountopen = 1,
                token = token
            };

            string checksum = $"{request.appid}{request.partnerid}{request.ceoname}{request.mobile}{request.telphonenumber}" +
                              $"{request.emailid}{request.pancard}{request.companyname}{request.address}{request.pincode}" +
                              $"{request.city}{request.state}{request.district}{request.dateofagreement}{request.channelid}" +
                              $"{request.channelkey}{request.custunqid}{request.callbackurl}{request.dmt}{request.aeps}" +
                              $"{request.cardpin}{request.accountopen}{token}";

            request.signcs = NsdlSignCsHelper.GenerateSignCs(key, checksum);

            using var client = new HttpClient();
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://jiffyuat.nsdlbank.co.in/jarvisgwy/bcregistration", content);
            response.EnsureSuccessStatusCode();

            var raw = await response.Content.ReadAsStringAsync();
            var registrationResponse = JsonSerializer.Deserialize<BcRegistrationResponse>(raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Access agentbcid safely from the response
            var Bcid = registrationResponse?.AgentData?.bcregistrationnewres?.bcid;

            // Return the agentbcid as a string (handle null if necessary)
            return Bcid?.ToString(); // If agentBcid is null, it will return null
        }
    }
}
