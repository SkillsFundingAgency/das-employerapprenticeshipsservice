using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [Route("api/ping")]
    public class PingController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { Hello = "World" });
        }
    }
}