using GIL_Agent_Portal.Models;
using GIL_Agent_Portal.Services.Intetrface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GIL_Agent_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BcAgentRegistrationController : ControllerBase
    {
        private readonly IBcAgentRegistrationService _service;

        public BcAgentRegistrationController(IBcAgentRegistrationService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAgent([FromBody] BcAgentRegistrationRequest model)
        {
            var response = await _service.RegisterAgentAsync(model);
            return Ok(response);
        }


    }
}
