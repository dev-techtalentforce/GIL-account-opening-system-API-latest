using GIL_Agent_Portal.Models;

namespace GIL_Agent_Portal.Repositories.Interface
{
    public interface IPaymentHistoryRepository
    {
        Task<int> AddPaymentHistory(PaymentHistoryResponse response);

    }
}
