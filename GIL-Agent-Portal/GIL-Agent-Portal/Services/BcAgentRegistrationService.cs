using GIL_Agent_Portal.Models;
using GIL_Agent_Portal.Repositories.Interface;
using GIL_Agent_Portal.Services.Intetrface;
using GIL_Agent_Portal.Utlity;
using Newtonsoft.Json;

namespace GIL_Agent_Portal.Services
{
    public class BcAgentRegistrationService : IBcAgentRegistrationService
    {
        private readonly IBcAgentRegistrationRepository _repository;
        private readonly SessionTokenService _sessionTokenService;
        private readonly NsdlSignCsHelper _nsdlSignCsHelper;
        private readonly NsdlBcRegistrationCaller _nsdlBcRegistrationCaller;
        public BcAgentRegistrationService(IBcAgentRegistrationRepository repository, SessionTokenService sessionTokenService, NsdlSignCsHelper nsdlSignCsHelper, NsdlBcRegistrationCaller nsdlBcRegistrationCaller)
        {
            _repository = repository;
            _sessionTokenService = sessionTokenService;
            _nsdlSignCsHelper = nsdlSignCsHelper;
            _nsdlBcRegistrationCaller = nsdlBcRegistrationCaller;
        }

        public async Task<BcAgentRegistrationResponse> RegisterAgentAsync(BcAgentRegistrationRequest model)
        {
            try
            {
                var tokenResponse = await _sessionTokenService.FetchNsdlSessionTokenAsync();
                string tokenKey = tokenResponse?.Sessiontokendtls?.TokenKey;

                if (string.IsNullOrWhiteSpace(tokenKey) || tokenKey.Length != 256)
                    throw new InvalidOperationException("Invalid or missing tokenKey. Expected 256 characters.");

                string token = NsdlSignCsHelper.ExtractToken(tokenKey);
                string secretKey = NsdlSignCsHelper.ExtractKey(tokenKey);

                string bcid = await _nsdlBcRegistrationCaller.CallBcRegistrationAsync();

                if (string.IsNullOrWhiteSpace(bcid))
                    throw new InvalidOperationException("Failed to retrieve BCID from NSDL registration.");

                model.token = token;
                model.bcid = bcid;
                model.agentbcid = bcid;

                model.dob = FormatDobAsDdMmYyyy(model.dob);

                string checksum = $"{model.channelid}{model.appid}{model.partnerid}{model.bcid}{model.bcagentid}{model.bcagentname}{model.middlename}{model.lastname}{model.companyname}" +
                                  $"{model.address}{model.statename}{model.cityname}{model.district}{model.area}{model.pincode}{model.mobilenumber}{model.telephone}{model.alternatenumber}{model.emailid}{model.dob}" +
                                  $"{model.shopaddress}{model.shopstate}{model.shopcity}{model.shopdistrict}{model.shoparea}{model.shoppincode}{model.pancard}{model.bcagentform}" +
                                  $"{model.productdetails?.dmt}{model.productdetails?.aeps}{model.productdetails?.cardpin}{model.productdetails?.accountopen}" +
                                  $"{model.terminaldetails?.tposserialno}{model.terminaldetails?.taddress}{model.terminaldetails?.taddress1}{model.terminaldetails?.tpincode}" +
                                  $"{model.terminaldetails?.tcity}{model.terminaldetails?.tstate}{model.terminaldetails?.temail}{model.agenttype}{model.agentbcid}{model.token}";

                model.signcs = NsdlSignCsHelper.GenerateSignCs(secretKey, checksum);

                string finalPayloadJson = JsonConvert.SerializeObject(model, Formatting.Indented);
                Console.WriteLine("📦 Final Agent Registration Payload:\n" + finalPayloadJson);

                return await _repository.SubmitAgentRegistrationAsync(model);
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"❗ HTTP error during registration: {httpEx.Message}");
                throw;
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"❗ JSON serialization error: {jsonEx.Message}");
                throw;
            }
            catch (InvalidOperationException invalidEx)
            {
                Console.WriteLine($"❗ Validation error: {invalidEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❗ Unexpected error: {ex.Message}");
                throw;
            }
        }

        public static string FormatDobAsDdMmYyyy(string dob)
        {
            if (string.IsNullOrWhiteSpace(dob)) return "";

            DateTime parsedDate;
            // Try all likely formats you expect, add more if needed
            string[] formats = { "yyyy-MM-dd", "MM/dd/yyyy", "dd/MM/yyyy", "dd-MM-yyyy" };

            if (DateTime.TryParseExact(dob, formats,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out parsedDate))
            {
                return parsedDate.ToString("dd/MM/yyyy");
            }
            else
            {
                throw new Exception($"DOB '{dob}' is not in a recognized format.");
            }
        }

        public Task<BcAgentRegistrationRequest> GetnsdlRegisterAgentById(string id)
        {
            var result = _repository.GetnsdlRegisterAgentById(id);

            return result;
        }
    }
}