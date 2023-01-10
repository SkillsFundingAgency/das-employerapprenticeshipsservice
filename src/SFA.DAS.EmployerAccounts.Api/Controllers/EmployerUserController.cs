using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EmployerAccounts.Api.Attributes;
using SFA.DAS.EmployerAccounts.Api.Orchestrators;

namespace SFA.DAS.EmployerAccounts.Api.Controllers
{
    [RoutePrefix("api/user/{userRef}")]
    public class EmployerUserController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        private readonly UsersOrchestrator _orchestrator;

        public EmployerUserController(UsersOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [Route("accounts", Name = "Accounts")]
        [ApiAuthorize(Roles = "ReadUserAccounts")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserAccounts(string userRef)
        {
            return Ok(await _orchestrator.GetUserAccounts(userRef));
        }
    }
}