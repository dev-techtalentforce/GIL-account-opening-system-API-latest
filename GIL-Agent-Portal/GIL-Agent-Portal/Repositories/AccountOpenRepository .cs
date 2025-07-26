
using GIL_Agent_Portal.DataContext;
using GIL_Agent_Portal.Models;
using GIL_Agent_Portal.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http.HttpResults;
using Dapper;
using GIL_Agent_Portal.DTOs;
using System.Data.Common;
using GIL_Agent_Portal.E;
using GIL_Agent_Portal.Utlity;
using GIL_Agent_Portal.Services;
using Newtonsoft.Json;
using System.Net;

namespace GIL_Agent_Portal.Repositories
{
    public class AccountOpenRepository : IAccountOpenRepository
    {
        private readonly string _connectionString;
        public AccountOpenRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task<int> InsertAsync(AccountOpenRequest req)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_AccountOpen_Insert", conn)
            { CommandType = CommandType.StoredProcedure };

            // Add parameters (exactly as in SP)
            cmd.Parameters.AddWithValue("@nomineeName", (object?)req.nomineeName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@nomineeDob", (object?)req.nomineeDob ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@relationship", (object?)req.relationship ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@add1", (object?)req.add1 ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@add2", (object?)req.add2 ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@add3", (object?)req.add3 ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@pin", (object?)req.pin ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@nomineeState", (object?)req.nomineeState ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@nomineeCity", (object?)req.nomineeCity ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@customername", (object?)req.customername ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@customerLastName", (object?)req.customerLastName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@dateofbirth", (object?)req.dateofbirth ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@pincode", (object?)req.pincode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@email", (object?)req.email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@mobileNo", (object?)req.mobileNo ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@maritalStatus", (object?)req.maritalStatus ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@income", (object?)req.income ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@middleNameOfMother", (object?)req.middleNameOfMother ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@houseOfFatherOrSpouse", (object?)req.houseOfFatherOrSpouse ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@kycFlag", (object?)req.kycFlag ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@panNo", (object?)req.panNo ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@channelid", (object?)req.channelid ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@partnerid", (object?)req.partnerid ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@applicationdocketnumber", (object?)req.applicationdocketnumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@dpid", (object?)req.dpid ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@clientid", (object?)req.clientid ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@tradingaccountnumber", (object?)req.tradingaccountnumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@partnerRefNumber", (object?)req.partnerRefNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@partnerpan", (object?)req.partnerpan ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@customerRefNumber", (object?)req.customerRefNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@customerDematId", (object?)req.customerDematId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@partnerCallBackURL", (object?)req.partnerCallBackURL ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@bcid", (object?)req.bcid ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@bcagentid", (object?)req.bcagentid ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@AgentId", (object?)req.AgentId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", req.Status);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<string> GenerateAccountOpenUrlAsync(AccountOpenRequest req)
        {
            try
            {
                // 1. Build JSON Payload
                var payload = new
                {
                    nomineeDetails = new
                    {
                        nomineeName = req.nomineeName,
                        nomineeDob = req.nomineeDob,
                        relationship = req.relationship,
                        add1 = req.add1,
                        add2 = req.add2,
                        add3 = req.add3,
                        pin = req.pin,
                        nomineeState = req.nomineeState,
                        nomineeCity = req.nomineeCity
                    },
                    personalDetails = new
                    {
                        customername = req.customername,
                        customerLastName = req.customerLastName,
                        dateofbirth = req.dateofbirth,
                        pincode = req.pincode,
                        email = req.email,
                        mobileNo = req.mobileNo
                    },
                    otherDetails = new
                    {
                        maritalStatus = req.maritalStatus,
                        income = req.income,
                        middleNameOfMother = req.middleNameOfMother,
                        houseOfFatherOrSpouse = req.houseOfFatherOrSpouse,
                        kycFlag = req.kycFlag,
                        panNo = req.panNo
                    },
                    additionalParameters = new
                    {
                        channelid = req.channelid,
                        partnerid = req.partnerid,
                        applicationdocketnumber = req.applicationdocketnumber,
                        dpid = req.dpid,
                        clientid = req.clientid,
                        partnerpan = req.partnerpan,
                        tradingaccountnumber = req.tradingaccountnumber,
                        partnerRefNumber = req.partnerRefNumber,
                        customerRefNumber = req.customerRefNumber,
                        customerDematId = req.customerDematId,
                        partnerCallBackURL = req.partnerCallBackURL,
                        bcid = req.bcid,
                        bcagentid = req.bcagentid
                    }
                };

                // 2. Encrypt the JSON string
                string jsonPayload = JsonConvert.SerializeObject(payload);
                string encryptedString = AesEncryptionHelper.Encrypt(jsonPayload);

                // 3. Fetch Session Token (asynchronously)
                var session = await new SessionTokenService().FetchNsdlSessionTokenAsync();
                if (session == null || string.IsNullOrEmpty(session.Sessiontokendtls?.TokenKey))
                {
                    throw new Exception("Failed to fetch session token or TokenKey is missing.");
                }

                string tokenKey = session.Sessiontokendtls.TokenKey;

                // 4. Generate SignCS (Signature)
                string token = NsdlSignCsHelper.ExtractToken(tokenKey);
                string key = NsdlSignCsHelper.ExtractKey(tokenKey);
                string signcs = NsdlSignCsHelper.GenerateSignCs(jsonPayload, key);

                // 5. URL Encode all values
                string urlEncEncrypted = WebUtility.UrlEncode(encryptedString);
                string urlEncSigncs = WebUtility.UrlEncode(signcs);
                string urlEncPartnerId = WebUtility.UrlEncode(req.partnerid);

                // 6. Build and return the URL
                string finalUrl = $"https://jiffyuat.nsdlbank.co.in/jarvisjiffytest/accountOpen" +
                                  $"?signcs={urlEncSigncs}" +
                                  $"&encryptedStringCustomer={urlEncEncrypted}" +
                                  $"&partnerid={urlEncPartnerId}";

                return finalUrl;
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw new Exception("An unexpected error occurred while generating the account URL.");
            }
        }




        public async Task<Dictionary<string, object>?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_AccountOpen_GetById", conn)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@AccountOpenId", id);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var result = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                    result[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                return result;
            }
            return null;
        }

        public async Task<List<Dictionary<string, object>>> GetAllAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_AccountOpen_GetAll", conn)
            { CommandType = CommandType.StoredProcedure };

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            var list = new List<Dictionary<string, object>>();
            while (await reader.ReadAsync())
            {
                var item = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                    item[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                list.Add(item);
            }
            return list;
        }

        public async Task<List<Dictionary<string, object>>> GetByAgentIdAsync(string agentId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_AccountOpen_GetByAgentId", conn)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@AgentId", agentId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            var list = new List<Dictionary<string, object>>();
            while (await reader.ReadAsync())
            {
                var item = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                    item[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                list.Add(item);
            }
            return list;
        }
    }
}
