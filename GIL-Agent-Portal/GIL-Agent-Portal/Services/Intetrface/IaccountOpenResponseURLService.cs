using GIL_Agent_Portal.Models;

namespace GIL_Agent_Portal.Services.Intetrface
{
    public interface IaccountOpenResponseURLService
    {
        Task<AccountOpenResponse> FetchUrlContentAsync(string targetUrl);
    }
}
