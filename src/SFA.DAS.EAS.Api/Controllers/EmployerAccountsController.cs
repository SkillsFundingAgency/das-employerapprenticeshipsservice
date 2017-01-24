using System.Net;
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
        [Authorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]   
        public async Task<IHttpActionResult> GetAccounts(string toDate = null, int pageSize = 1000, int pageNumber = 1)
        {
            var result = await _orchestrator.GetAllAccountsWithBalances(toDate, pageSize, pageNumber);
            
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

        [Route("{hashedId}", Name = "GetAccount")]
        [Authorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAccount(string hashedAccountId)
        {
            return NotFound();
        }

        [Route("{hashedId}/legalentities", Name = "GetLegalEntities")]
        [Authorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLegalEntities(string hashedAccountId)
        {
            return NotFound();
        }

        [Route("{hashedId}/legalentities/{legalentityid}", Name = "GetLegalEntity")]
        [Authorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLegalEntity(string hashedAccountId, long legalEntityId)
        {
            return NotFound();
        }

        [Route("{hashedId}/payeschemes", Name = "GetPayeSchemes")]
        [Authorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPayeSchemes(string hashedAccountId)
        {
            return NotFound();
        }

        [Route("{hashedId}/payeschemes/{payeschemeref}", Name = "GetPayeScheme")]
        [Authorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPayeScheme(string hashedAccountId, string payeSchemeRef)
        {
            return NotFound();
        }
    }
}
