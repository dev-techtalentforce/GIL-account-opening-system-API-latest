using GIL_Agent_Portal.Models;

namespace GIL_Agent_Portal.Repositories.Interface
{
    public interface IAccountOpenRepository
    {
        Task<int> InsertAsync(AccountOpenRequest req);
        Task<Dictionary<string, object>?> GetByIdAsync(int id);
        Task<List<Dictionary<string, object>>> GetAllAsync();
        Task<List<Dictionary<string, object>>> GetByAgentIdAsync(string agentId);
    }
}
