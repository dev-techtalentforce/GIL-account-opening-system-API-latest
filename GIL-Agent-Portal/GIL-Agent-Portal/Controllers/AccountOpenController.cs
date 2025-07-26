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

        public AccountOpenController(IAccountOpenRepository repo)
        {
            _repo = repo;
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
            return Ok(url);
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
    }
}
