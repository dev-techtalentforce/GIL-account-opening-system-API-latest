using GIL_Agent_Portal.Models;

namespace GIL_Agent_Portal.Repositories.Interface
{
    public interface IBcAgentRegistrationRepository
    {
        Task<BcAgentRegistrationResponse> SubmitAgentRegistrationAsync(BcAgentRegistrationRequest model);
        Task<BcAgentRegistrationRequest> GetnsdlRegisterAgentById(string id);

    }
}
