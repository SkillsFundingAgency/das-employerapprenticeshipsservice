﻿using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EmployerAccounts.Api.Attributes;
using SFA.DAS.EmployerAccounts.Api.Orchestrators;

namespace SFA.DAS.EmployerAccounts.Api.Controllers
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
            var result = await _orchestrator.GetUserAccounts(userRef);

            if (result.Status == HttpStatusCode.OK)
            {
                return Ok(result.Data);
            }
            
            return Conflict();
        }
    }
}