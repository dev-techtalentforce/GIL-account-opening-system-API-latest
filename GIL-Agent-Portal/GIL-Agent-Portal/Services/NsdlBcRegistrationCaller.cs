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
                try
                {
                    // Fetch the session token.
                    var session = await new SessionTokenService().FetchNsdlSessionTokenAsync();
                    string tokenKey = session.Sessiontokendtls.TokenKey;

                    // Extract token and key from the session token.
                    string token = NsdlSignCsHelper.ExtractToken(tokenKey);
                    string key = NsdlSignCsHelper.ExtractKey(tokenKey);

                    // Prepare the registration request.
                    var request = new BcRegistrationRequest
                    {
                        appid = "com.jarviswebbc.nsdlpb",
                        partnerid = "wpemmjhKus",
                        ceoname = "nimesh patel",
                        mobile = "9924142245",
                        telphonenumber = "07927485100",
                        emailid = "nimeshpatel7@gamil.com",
                        pancard = "UWPCL6780T",
                        companyname = "tech telent force",
                        address= "A2 2nd Floor Jay Tower Ankur Complex Near Ankur Bus Stop Naranpura Ahmedabad 380013 Gujarat",
                        pincode = "380080",
                        city = "Ahmedabad",
                        state = "Ahmedabad",
                        district = "Ahmedabad",
                        dateofagreement = "08/28/2024",
                        channelid = "lfbpWjegXHwnnirQOlYP",
                        channelkey = "R2gveNNtRmxQrucvkztkwoucaf8agYmVYNyeFi3JDenX7HdzRo9scFh4Usn8nqllgGbeoODhag3aiuCrlPucswV9COBm3bkSGfjmLmQw3gLx3SdEuaxBZnsgrczI5B8V",
                        custunqid = "wpemmjhKus",
                        callbackurl = "nsdl.gujaratinfotech.com",
                        dmt = 1,
                        aeps = 1,
                        cardpin = 0,
                        accountopen = 1,
                        token = token // Use the actual token here
                    };



                    // Prepare checksum string based on the registration fields.
                    string checksum = $"{request.appid}{request.partnerid}{request.ceoname}{request.mobile}{request.telphonenumber}" +
                                      $"{request.emailid}{request.pancard}{request.companyname}{request.address}{request.pincode}" +
                                      $"{request.city}{request.state}{request.district}{request.dateofagreement}{request.channelid}" +
                                      $"{request.channelkey}{request.custunqid}{request.callbackurl}{request.dmt}{request.aeps}" +
                                      $"{request.cardpin}{request.accountopen}{request.token}";

                    // Generate the signcs (checksum) using the key and checksum string.
                    request.signcs = NsdlSignCsHelper.GenerateSignCs(key, checksum);

                    // Send the registration request via HTTP.
                    using var client = new HttpClient();
                    var json = JsonSerializer.Serialize(request);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("https://jiffyuat.nsdlbank.co.in/jarvisgwy/bcregistration", content);
                    response.EnsureSuccessStatusCode();

                    // Read the response from the registration API.
                    var raw = await response.Content.ReadAsStringAsync();
                    var registrationResponse = JsonSerializer.Deserialize<BcRegistrationResponse>(raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Access agentbcid safely from the response.
                //var Bcid = registrationResponse?.AgentData?.bcregistrationnewres?.bcid;
                var Bcid = "1577";

                    // Return the agentbcid as a string (handle null if necessary).
                    return Bcid?.ToString(); // If agentBcid is null, it will return null.
                }
                catch (HttpRequestException httpEx)
                {
                    // Log the HTTP error
                    Console.WriteLine($"❗ HTTP request error: {httpEx.Message}");
                }
                catch (JsonException jsonEx)
                {
                    // Log the JSON parsing error
                    Console.WriteLine($"❗ JSON parsing error: {jsonEx.Message}");
                }
                catch (NullReferenceException nullEx)
                {
                    // Log the NullReferenceException error
                    Console.WriteLine($"❗ Null reference error: {nullEx.Message}");
                }
                catch (Exception ex)
                {
                    // Log any other unexpected error
                    Console.WriteLine($"❗ Unexpected error: {ex.Message}");
                }

                return null;
            }
        }
    
}
