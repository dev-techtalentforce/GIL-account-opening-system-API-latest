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

                //actual checksum......
                //string checksum = $"{model.channelid}{model.appid}{model.partnerid}{model.bcid}{model.bcagentid}{model.bcagentname}{model.middlename}{model.lastname}{model.companyname}" +
                //                  $"{model.address}{model.statename}{model.cityname}{model.district}{model.area}{model.pincode}{model.mobilenumber}{model.telephone}{model.alternatenumber}{model.emailid}{model.dob}" +
                //                  $"{model.shopaddress}{model.shopstate}{model.shopcity}{model.shopdistrict}{model.shoparea}{model.shoppincode}{model.pancard}{model.bcagentform}" +
                //                  $"{model.productdetails?.dmt}{model.productdetails?.aeps}{model.productdetails?.cardpin}{model.productdetails?.accountopen}" +
                //                  $"{model.terminaldetails?.tposserialno}{model.terminaldetails?.taddress}{model.terminaldetails?.taddress1}{model.terminaldetails?.tpincode}" +
                //                  $"{model.terminaldetails?.tcity}{model.terminaldetails?.tstate}{model.terminaldetails?.temail}{model.agenttype}{model.agentbcid}{model.token}";

                //string checksum = $"{model.channelid}{model.appid}{model.partnerid}{model.bcid}{model.bcagentid}{model.bcagentname}{model.middlename}{model.lastname}{model.companyname}" +
                //         $"{model.address}{model.statename}{model.cityname}{model.district}{model.area}{model.pincode}{model.mobilenumber}{model.telephone}{model.alternatenumber}{model.emailid}{model.dob}" +
                //         $"{model.shopaddress}{model.shopstate}{model.shopcity}{model.shopdistrict}{model.shoparea}{model.shoppincode}{model.pancard}{model.bcagentform}" +
                //         $"{model.productdetails?.dmt ?? ""}{model.productdetails?.aeps ?? ""}{model.productdetails?.cardpin ?? ""}{model.productdetails?.accountopen ?? ""}" +
                //         $"{model.terminaldetails?.tposserialno ?? ""}{model.terminaldetails?.taddress ?? ""}{model.terminaldetails?.taddress1 ?? ""}{model.terminaldetails?.tpincode ?? ""}" +
                //         $"{model.terminaldetails?.tcity ?? ""}{model.terminaldetails?.tstate ?? ""}{model.terminaldetails?.temail ?? ""}{model.agenttype}{model.agentbcid}{model.token}";

                string checksum = $"{model.channelid?.Trim()}{model.appid?.Trim()}{model.partnerid?.Trim()}{model.bcid?.Trim()}{model.bcagentid?.Trim()}{model.bcagentname?.Trim()}{model.middlename?.Trim()}{model.lastname?.Trim()}{model.companyname?.Trim()}" +
                  $"{model.address?.Trim()}{model.statename?.Trim()}{model.cityname?.Trim()}{model.district?.Trim()}{model.area?.Trim()}{model.pincode?.Trim()}{model.mobilenumber?.Trim()}{model.telephone?.Trim()}{model.alternatenumber?.Trim()}{model.emailid?.Trim()}{model.dob?.Trim()}" +
                  $"{model.shopaddress?.Trim()}{model.shopstate?.Trim()}{model.shopcity?.Trim()}{model.shopdistrict?.Trim()}{model.shoparea?.Trim()}{model.shoppincode?.Trim()}{model.pancard?.Trim()}{model.bcagentform?.Trim()}" +
                  $"{model.productdetails?.dmt?.Trim() ?? ""}{model.productdetails?.aeps?.Trim() ?? ""}{model.productdetails?.cardpin?.Trim() ?? ""}{model.productdetails?.accountopen?.Trim() ?? ""}" +
                  $"{model.terminaldetails?.tposserialno?.Trim() ?? ""}{model.terminaldetails?.taddress?.Trim() ?? ""}{model.terminaldetails?.taddress1?.Trim() ?? ""}{model.terminaldetails?.tpincode?.Trim() ?? ""}" +
                  $"{model.terminaldetails?.tcity?.Trim() ?? ""}{model.terminaldetails?.tstate?.Trim() ?? ""}{model.terminaldetails?.temail?.Trim() ?? ""}{model.agenttype?.Trim()}{model.agentbcid?.Trim()}{model.token?.Trim()}";



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
    }
}