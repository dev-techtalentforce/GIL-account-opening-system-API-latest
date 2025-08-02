using GIL_Agent_Portal.Models;
using Razorpay.Api;

namespace GIL_Agent_Portal.Services.Intetrface
{
    public interface IBcAgentRegistrationService
    {
        Task<BcAgentRegistrationResponse> RegisterAgentAsync(BcAgentRegistrationRequest model);
        Task<BcAgentRegistrationRequest> GetnsdlRegisterAgentById(string id);
    }
}
