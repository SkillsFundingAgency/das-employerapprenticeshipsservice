using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.EmployerAccounts.Api.Controllers
{
    [Route("ping")]
    public class PingController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}