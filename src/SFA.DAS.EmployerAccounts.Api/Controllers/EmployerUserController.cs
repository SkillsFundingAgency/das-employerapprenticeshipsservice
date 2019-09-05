using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SFA.DAS.EmployerAccounts.Api.Attributes;
using SFA.DAS.EmployerAccounts.Api.Orchestrators;

namespace SFA.DAS.EmployerAccounts.Api.Controllers
{
    [System.Web.Http.RoutePrefix("api/user/{userRef}")]
    public class EmployerUserController : ApiController
    {
        private readonly UsersOrchestrator _orchestrator;

        public EmployerUserController(UsersOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [System.Web.Http.Route("accounts", Name = "Accounts")]
        [ApiAuthorize(Roles = "ReadUserAccounts")]
        [System.Web.Http.HttpGet]
        public async Task<IHttpActionResult> GetUserAccounts(string userRef)
        {
            return Ok(await _orchestrator.GetUserAccounts(userRef));
        }
    }
}