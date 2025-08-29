using GIL_Agent_Portal.E;
using GIL_Agent_Portal.Models;
using GIL_Agent_Portal.Repositories.Interface;
using GIL_Agent_Portal.Services.Intetrface;
using Newtonsoft.Json;
using System.Net.Http;
using System.Web;

namespace GIL_Agent_Portal.Services
{
    public class AccountOpenResponseURLService : IaccountOpenResponseURLService
    {
        private readonly HttpClient _httpClient;
        private readonly IAccountOpenRepository _AccountOpenrepository;

        public AccountOpenResponseURLService(HttpClient httpClient, IAccountOpenRepository AccountOpenrepository)
        {
            _httpClient = httpClient;
            _AccountOpenrepository = AccountOpenrepository;
        }

        //public string EncryptCustomerData(object customerObj)
        //{
        //    // Serialize object to JSON or combine as per NSDL requirement
        //    string combined = $"{customerObj.AccountNumber}{customerObj.CustomerId}..."; // in required order
        //    return AesEncryptionHelper.Encrypt(combined);
        //}

        //public string GenerateRedirectUrl(AccountOpenResponse resp, string partnerCallbackUrl, string appSignKey)
        //{
        //    // Create encreqdetails in required order
        //    string encrQDtls = AesEncryptionHelper.Encrypt(/* ...combined string... */);

        //    // SignCs on the SAME combined string
        //    string signCs = AesEncryptionHelper.GenerateSignCs(/* same string */, appSignKey);

        //    // Compose redirect URL
        //    string url = $"{partnerCallbackUrl}?response={Uri.EscapeDataString(resp.Response)}&respcode={resp.RespCode}&encreqdetails={Uri.EscapeDataString(encrQDtls)}&signcs={Uri.EscapeDataString(signCs)}";
        //    return url;
        //}
        public async Task<AccountOpenResponse> FetchUrlContentAsync(string targetUrl)
        {
            if (string.IsNullOrWhiteSpace(targetUrl))
                throw new ArgumentException("Target URL cannot be empty.");

            if (!Uri.TryCreate(targetUrl, UriKind.RelativeOrAbsolute, out var uri))
                throw new UriFormatException("Invalid URL format.");

            var queryParams = HttpUtility.ParseQueryString(uri.Query);
            if (queryParams == null)
                throw new InvalidOperationException("Query parameters could not be parsed.");

            var encryptedData = queryParams["encreqdetails"];
            var decryptedJson = AesEncryptionHelper.Decrypt(encryptedData);
            if (string.IsNullOrWhiteSpace(encryptedData))
                throw new InvalidOperationException("Missing 'encreqdetails' in URL.");

            // URL decode before decryption
            var decoded = HttpUtility.UrlDecode(encryptedData);

            // Decrypt using your helper
            //var decryptedJson = AesEncryptionHelper.Decrypt(decoded);

            // Deserialize to payload
            var payload = JsonConvert.DeserializeObject<AccountOpenResponse>(decryptedJson);

            await _AccountOpenrepository.InsertAccountOpenResponseAsync(payload);
            return payload;
        }

    }
}


