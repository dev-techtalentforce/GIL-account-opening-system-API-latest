using Dapper;
using GIL_Agent_Portal.Models;
using GIL_Agent_Portal.Repositories.Interface;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.SqlClient;
using static GIL_Agent_Portal.Utlity.RazorPayService;

namespace GIL_Agent_Portal.Repositories
{
    public class PaymentHistoryRepository : IPaymentHistoryRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly IDbConnection _dbConnection;

        public PaymentHistoryRepository(IConfiguration configuration, IDbConnection dbConnection, ILogger<UsersRepository> logger)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection"); // update name if different
            _dbConnection = dbConnection;
        }

        public async Task<int> AddPaymentHistory(PaymentHistoryResponse response)
        {
            var sp = "InsertPaymentHistory";
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@PhOrderId", response.PhOrderId);
                    parameters.Add("@PhPaidAmount", response.PhPaidAmount);
                    parameters.Add("@PhStatus", response.PhStatus);
                    parameters.Add("@PhCreditId", response.PhCreditId ?? (object)DBNull.Value);
                    parameters.Add("@PhCreditValue", response.PhCreditValue ?? (object)DBNull.Value);

                    // Use ExecuteScalarAsync for single value
                    var result = await db.ExecuteScalarAsync<int>(sp, parameters, commandType: CommandType.StoredProcedure);
                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error retrieving Razorpay payments: {ex.Message}", ex);
                }
            }
        }

    }
}
