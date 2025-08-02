using GIL_Agent_Portal.Models;
using GIL_Agent_Portal.Repositories.Interface;
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
        private readonly IUsersRepository _usersRepository;

        public BcAgentRegistrationController(IBcAgentRegistrationService service, IUsersRepository usersRepository)
        {
            _service = service;
            _usersRepository = usersRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAgent([FromBody] BcAgentRegistrationRequest model)
        {
            var response = await _service.RegisterAgentAsync(model);

            var status = response?.AgentData?.bcagentregistrationres?.status;
            if (status == 1)
            {
                var updateUser = new updateUser
                {
                    UserId = response?.AgentData.bcagentregistrationres.bcagentid, 
                    nsdl_status = 1
                };
                var updatedUser = _usersRepository.UserUpdate(updateUser);

          
            }
            return Ok(response);
        }

        [HttpGet("AgentId/{id}")]
        public async Task<IActionResult> GetnsdlRegisterAgentById([FromRoute] string id)
        {
            // Fetch the record from the repository by ID
            var account = await _service.GetnsdlRegisterAgentById(id);

            if (account == null)
            {
                // Return NotFound if no account is found
                return NotFound(new { message = "Account not found" });
            }

            // Return Ok with the account data
            return Ok(account);
        }


    }
}
