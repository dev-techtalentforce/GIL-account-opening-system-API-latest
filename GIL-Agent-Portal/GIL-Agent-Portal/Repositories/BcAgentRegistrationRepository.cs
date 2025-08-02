using GIL_Agent_Portal.Models;
using System.Text.Json;
using System.Text;
using GIL_Agent_Portal.Repositories.Interface;
using System.Data;
using Dapper;
using Razorpay.Api;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Data.SqlClient;

namespace GIL_Agent_Portal.Repositories
{
    public class BcAgentRegistrationRepository : IBcAgentRegistrationRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly IDbConnection _dbConnection;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUsersRepository _userRepository;


        public BcAgentRegistrationRepository(IConfiguration configuration, IDbConnection dbConnection, IHttpClientFactory httpClientFactory, IUsersRepository usersRepository )
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection"); // Ensure connection string name matches
            _dbConnection = dbConnection;
            _httpClientFactory = httpClientFactory;
            _userRepository = usersRepository;
        }


        public async Task<BcAgentRegistrationResponse> SubmitAgentRegistrationAsync(BcAgentRegistrationRequest model)
        {
            // 1. Call external API
            var client = _httpClientFactory.CreateClient();
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://jiffyuat.nsdlbank.co.in/jarvisgwy/partner/bcagentregistration", content);
            response.EnsureSuccessStatusCode();

            var raw = await response.Content.ReadAsStringAsync();

            // 2. Deserialize API response
            var apiResponse = JsonSerializer.Deserialize<BcAgentRegistrationResponse>(
                raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (apiResponse.respcode == "00")
            {

                // 3. Log request and response in DB
                var parameters = new DynamicParameters();
                parameters.Add("@ChannelId", model.channelid);
                parameters.Add("@AppId", model.appid);
                parameters.Add("@PartnerId", model.partnerid);
                parameters.Add("@BcId", model.bcid);
                parameters.Add("@BcAgentId", model.bcagentid);
                parameters.Add("@BcAgentName", model.bcagentname);
                parameters.Add("@MiddleName", model.middlename);
                parameters.Add("@LastName", model.lastname);
                parameters.Add("@CompanyName", model.companyname);
                parameters.Add("@Address", model.address);
                parameters.Add("@StateName", model.statename);
                parameters.Add("@CityName", model.cityname);
                parameters.Add("@District", model.district);
                parameters.Add("@Area", model.area);
                parameters.Add("@Pincode", model.pincode);
                parameters.Add("@MobileNumber", model.mobilenumber);
                parameters.Add("@Telephone", model.telephone);
                parameters.Add("@AlternateNumber", model.alternatenumber);
                parameters.Add("@EmailId", model.emailid);
                parameters.Add("@DOB", model.dob);
                parameters.Add("@ShopAddress", model.shopaddress);
                parameters.Add("@ShopState", model.shopstate);
                parameters.Add("@ShopCity", model.shopcity);
                parameters.Add("@ShopDistrict", model.shopdistrict);
                parameters.Add("@ShopArea", model.shoparea);
                parameters.Add("@ShopPincode", model.shoppincode);
                parameters.Add("@PanCard", model.pancard);
                parameters.Add("@BcAgentForm", model.bcagentform);

                // Product Details (BIT)
                parameters.Add("@Product_DMT", model.productdetails?.dmt == "1" || model.productdetails?.dmt?.ToLower() == "true");
                parameters.Add("@Product_AEPS", model.productdetails?.aeps == "1" || model.productdetails?.aeps?.ToLower() == "true");
                parameters.Add("@Product_CardPin", model.productdetails?.cardpin == "1" || model.productdetails?.cardpin?.ToLower() == "true");
                parameters.Add("@Product_AccountOpen", model.productdetails?.accountopen == "1" || model.productdetails?.accountopen?.ToLower() == "true");

                // Terminal Details
                parameters.Add("@Terminal_TposSerialNo", model.terminaldetails?.tposserialno);
                parameters.Add("@Terminal_TAddress", model.terminaldetails?.taddress);
                parameters.Add("@Terminal_TAddress1", model.terminaldetails?.taddress1);
                parameters.Add("@Terminal_TPincode", model.terminaldetails?.tpincode);
                parameters.Add("@Terminal_TCity", model.terminaldetails?.tcity);
                parameters.Add("@Terminal_TState", model.terminaldetails?.tstate);
                parameters.Add("@Terminal_TEmail", model.terminaldetails?.temail);

                parameters.Add("@AgentType", model.agenttype);
                parameters.Add("@AgentBcId", model.agentbcid);
                parameters.Add("@Token", model.token);
                parameters.Add("@SignCS", model.signcs);

                // Store raw JSON response as log
                parameters.Add("@ResponseJson", raw);


                var data = new updateUser
                {
                    UserId = model.bcagentid, // Assuming bcagentid is the UserId
                    status = null,
                    BlockStatus = null,
                    nsdl_status = 1,

                };

                _userRepository.UserUpdate(data);

                await _dbConnection.ExecuteAsync(
                    "BcAgentRegistration_insert", parameters, commandType: CommandType.StoredProcedure
                );

                return apiResponse!;

            }
            else
            {
                return apiResponse!;

            }
            // 4. Return the API response
            return apiResponse!;
        }

        public async Task<BcAgentRegistrationRequest> GetnsdlRegisterAgentById(string id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();

                var parameters = new DynamicParameters();
                parameters.Add("@AgentId", id);

                // Call the stored procedure or query to fetch the agent
                var result = await db.QueryFirstOrDefaultAsync<BcAgentRegistrationRequest>(
                    "GetnsdlRegisterAgentById",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
        }

    }





    //public async Task<BcAgentRegistrationResponse> SubmitAgentRegistrationAsync(BcAgentRegistrationRequest model)
    //{
    //    using var client = new HttpClient();
    //    var json = JsonSerializer.Serialize(model);
    //    var content = new StringContent(json, Encoding.UTF8, "application/json");

    //    var response = await client.PostAsync("https://jiffyuat.nsdlbank.co.in/jarvisgwy/partner/bcagentregistration", content);
    //    response.EnsureSuccessStatusCode();

    //    var raw = await response.Content.ReadAsStringAsync();
    //    return JsonSerializer.Deserialize<BcAgentRegistrationResponse>(raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    //}



}
