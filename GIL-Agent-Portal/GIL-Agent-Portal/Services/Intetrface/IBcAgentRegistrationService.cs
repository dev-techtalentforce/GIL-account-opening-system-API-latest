using GIL_Agent_Portal.Models;

namespace GIL_Agent_Portal.Services.Intetrface
{
    public interface IBcAgentRegistrationService
    {
        Task<BcAgentRegistrationResponse> RegisterAgentAsync(BcAgentRegistrationRequest model);
    }
}
