using Dapper;
using GIL_Agent_Portal.Models;
using System.Data;

namespace GIL_Agent_Portal.Repositories
{
    public class RazorPayRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<RazorPayRepository> _logger;

        public RazorPayRepository(IDbConnection dbConnection, ILogger<RazorPayRepository> logger)
        {
            _dbConnection = dbConnection;
            _logger = logger;
        }


    public bool SavePaymentToDB(RZPCheckoutPayment payment, string orderID, string requestJson, string responseJson)
        {
            var sp = "sp_InsertRazorPayPayment";
            var parameters = new DynamicParameters();   

            parameters.Add("@Amount", payment.amount);
            parameters.Add("@BusinessId", payment.businessID);
            parameters.Add("@PlanId", payment.planID);
            parameters.Add("@ReceiptId", payment.receipt);
            parameters.Add("@OrderId", payment.orderID);
            parameters.Add("@TransId", payment.transID ?? "");
            parameters.Add("@PaymentStatus", payment.paymentStatus);
            parameters.Add("@CreatedAt", DateTime.Now);
            parameters.Add("@RequestData", payment.RequestData);
            parameters.Add("@ResponseData", payment.ResponseData);

            try
            {
                if (_dbConnection.State == ConnectionState.Closed)
                    _dbConnection.Open();

                var result = _dbConnection.Execute(sp, parameters, commandType: CommandType.StoredProcedure);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving RazorPay payment to DB");
                return false;
            }
            finally
            {
                if (_dbConnection.State == ConnectionState.Open)
                    _dbConnection.Close();
            }
        }
    }
}
