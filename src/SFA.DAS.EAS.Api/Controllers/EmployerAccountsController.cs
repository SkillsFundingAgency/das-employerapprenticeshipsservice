using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EAS.Api.Orchestrators;

namespace SFA.DAS.EAS.Api.Controllers
{
    [RoutePrefix("api/accounts")]
    public class EmployerAccountsController : ApiController
    {
        private readonly AccountsOrchestrator _orchestrator;

        public EmployerAccountsController(AccountsOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [Route("", Name = "AccountsIndex")]
        [HttpGet]
        public async Task<IHttpActionResult> Index(string fromDate = null, int pageSize = 1000, int pageNumber = 1)
        {
            var result = await _orchestrator.GetAllAccountsWithBalances(fromDate, pageSize, pageNumber);
            
            if (result.Status == HttpStatusCode.OK)
            {
                return Ok(result.Data);
            }
            else
            {
                //TODO: Handle unhappy paths.
                return Conflict();
            }
        }
    }
}
