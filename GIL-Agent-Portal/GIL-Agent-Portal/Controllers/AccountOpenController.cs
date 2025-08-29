using GIL_Agent_Portal.E;
using GIL_Agent_Portal.Models;
using GIL_Agent_Portal.Repositories.Interface;
using GIL_Agent_Portal.Services.Intetrface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GIL_Agent_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountOpenController : ControllerBase
    {
        private readonly IAccountOpenRepository _repo;
        private readonly IaccountOpenResponseURLService _accountOpenService;
        //private readonly AesEncryptionHelper _aesEncryptionHelper;

        public AccountOpenController(IAccountOpenRepository repo, IaccountOpenResponseURLService accountOpenService)
        {
            _repo = repo;
            _accountOpenService = accountOpenService;
            //_aesEncryptionHelper = aesEncryptionHelper;
        } 

        // POST: api/AccountOpen/insert
        [HttpPost("insert")]
        public async Task<IActionResult> Insert([FromBody] AccountOpenRequest req)
        {   
            var id = await _repo.InsertAsync(req);
            // return Ok(new { AccountOpenId = id });
            // 2. Generate the NSDL account open URL
            string url =await  _repo.GenerateAccountOpenUrlAsync(req);

            // 3. Return only the URL (plain string or as a property)
            //return Ok(url);
            return Ok(new { URL = url });
        }

        // GET: api/AccountOpen/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var record = await _repo.GetByIdAsync(id);
            if (record == null)
                return NotFound();
            return Ok(record);
        }

        // GET: api/AccountOpen/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _repo.GetAllAsync();
            return Ok(list);
        }

        // GET: api/AccountOpen/agent/{agentId}
        [HttpGet("agent/{agentId}")]
        public async Task<IActionResult> GetByAgentId(string agentId)
        {
            var list = await _repo.GetByAgentIdAsync(agentId);
            return Ok(list);
        }



        [HttpGet("AccountOpenResponse")]
        public async Task<IActionResult> GetUrlContent([FromQuery] string targetUrl)
        {
            if (string.IsNullOrWhiteSpace(targetUrl))
                return BadRequest("Target URL is required.");

            try
            {
                var content = await _accountOpenService.FetchUrlContentAsync(targetUrl);
                return Ok(new { url = targetUrl, content });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(502, $"External request failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }



        //response to redirected account open

        //[HttpPost("open")]
        //public async Task<IActionResult> OpenAccount([FromBody] AccountOpenResponse model, PartnerRedirectResult fullURL)
        //{
        //    // 1. Compose data for encryption and signature (as per NSDL doc - strict order!)
        //    string dataToEncrypt = $"{model.AccountNumber}{model.CustomerId}{model.Cif}{model.CustomerName}{model.CustomerLastName}{model.Email}{model.MobileNo}{model.PartnerId}{model.ChannelId}{model.PartnerRefNumber}{model.CustomerRefNumber}{model.CustomerDematId}{model.CustomerClientId}{model.Response}{model.RespCode}{model.BranchCode}{model.BranchName}{model.Category}{model.Ifsccode}";

        //    // 2. Encrypt the data
        //    string encrQDtls = AesEncryptionHelper.Encrypt(dataToEncrypt);

        //    // 3. Generate the SignCs
        //    string signCs = AesEncryptionHelper.GenerateSignCs(dataToEncrypt);

        //    // 4. Store data in database (example, expand fields as needed)
        //    await _accountOpenService.PreparePartnerRedirect(dataToEncrypt);

        //    // 5. Prepare redirect URL (as per partner requirements)
        //    string partnerCallbackUrl = fullURL.FullUrl; // Should be part of the model/request
        //    string url = $"{partnerCallbackUrl}?response={Uri.EscapeDataString(model.Response)}&respcode={model.RespCode}&encreqdetails={Uri.EscapeDataString(encrQDtls)}&signcs={Uri.EscapeDataString(signCs)}";

        //    // 6. Optionally: call NSDL endpoint here if needed

        //    // 7. Return URL to frontend
        //    return Ok(new { redirectUrl = url });
        //}

        //public PartnerCustomerModel MapToPartnerCustomer(string decrypted, string response, string respcode)
        //{
        //    // Example mapping logic
        //    var model = new PartnerCustomerModel
        //    {
        //        DecryptedData = decrypted,
        //        ResponseMessage = response,
        //        ResponseCode = respcode
        //    };

        //    return model;
        //}

    }
}
