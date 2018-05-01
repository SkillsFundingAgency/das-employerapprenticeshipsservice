using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Account.Api.Helpers;
using SFA.DAS.EAS.Account.Api.Orchestrators;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [RoutePrefix("api/user/{userRef}")]
    public class EmployerUserController : ApiController
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
            var result = await _orchestrator.GetUserAccounts(GetUserRefAsGuId(userRef));

            if (result.Status == HttpStatusCode.OK)
            {
                return Ok(result.Data);
            }
            
            //TODO: Handle unhappy paths.
            return Conflict();
        }

        private Guid GetUserRefAsGuId(string userRef)
        {
            return Guid.TryParse(userRef, out var userIdGuid) ? userIdGuid : Guid.Empty;
        }
    }
}