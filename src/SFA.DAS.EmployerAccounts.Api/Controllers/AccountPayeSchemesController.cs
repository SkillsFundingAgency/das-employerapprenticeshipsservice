﻿using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SFA.DAS.EmployerAccounts.Api.Attributes;
using SFA.DAS.EmployerAccounts.Api.Orchestrators;

namespace SFA.DAS.EmployerAccounts.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/payeschemes")]
    public class AccountPayeSchemesController : ApiController
    {
        private readonly AccountsOrchestrator _orchestrator;

        public AccountPayeSchemesController(AccountsOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [Route("{payeschemeref}", Name = "GetPayeScheme")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPayeScheme(string hashedAccountId, string payeSchemeRef)
        {
            var result = await _orchestrator.GetPayeScheme(hashedAccountId, HttpUtility.UrlDecode(payeSchemeRef));

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}