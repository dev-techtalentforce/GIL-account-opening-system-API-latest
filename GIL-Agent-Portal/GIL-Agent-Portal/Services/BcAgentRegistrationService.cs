using GIL_Agent_Portal.Models;
using GIL_Agent_Portal.Repositories.Interface;
using GIL_Agent_Portal.Services.Intetrface;
using GIL_Agent_Portal.Utlity;

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


           
            var tokenResponse = await _sessionTokenService.FetchNsdlSessionTokenAsync();

            
            if (tokenResponse?.Sessiontokendtls?.Token == null)
            {
               
                throw new InvalidOperationException("Failed to fetch session token.");
            }

            string token = tokenResponse.Sessiontokendtls.Token;

            
            string signcs = await _nsdlSignCsHelper.RunTestAsync();  // Now this returns the signcs

           
            string agentbcid = await _nsdlBcRegistrationCaller.CallBcRegistrationAsync();  // Now this returns the agentbcid

           
            model.token = token;
            model.signcs = signcs; 
            model.agentbcid = agentbcid; 

            string checksum = $"{model.channelid}{model.appid}{model.partnerid}{model.bcid}{model.bcagentid}{model.bcagentname}{model.middlename}{model.lastname}{model.companyname}" +
                              $"{model.address}{model.statename}{model.cityname}{model.district}{model.area}{model.pincode}{model.mobilenumber}{model.telephone}{model.alternatenumber}{model.emailid}{model.dob}" +
                              $"{model.shopaddress}{model.shopstate}{model.shopcity}{model.shopdistrict}{model.shoparea}{model.shoppincode}{model.pancard}{model.bcagentform}" +
                              $"{model.productdetails.dmt}{model.productdetails.aeps}{model.productdetails.cardpin}{model.productdetails.accountopen}" +
                              $"{model.terminaldetails.tposserialno}{model.terminaldetails.taddress}{model.terminaldetails.taddress1}{model.terminaldetails.tpincode}" +
                              $"{model.terminaldetails.tcity}{model.terminaldetails.tstate}{model.terminaldetails.temail}{model.agenttype}{model.agentbcid}{model.token}";

           
            model.signcs = NsdlSignCsHelper.GenerateSignCs(NsdlSignCsHelper.ExtractKey(model.token), checksum);

            Console.WriteLine("BCAGENTREGISTRATION..", checksum);

           
            return await _repository.SubmitAgentRegistrationAsync(model);
        }
    }
}