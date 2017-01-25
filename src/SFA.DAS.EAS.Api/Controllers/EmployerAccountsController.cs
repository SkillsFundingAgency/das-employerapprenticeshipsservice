using System.Net;
using System.Threading.Tasks;
using System.Web;
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
                result.Data.Data.ForEach(x => x.Href = Url.Link("GetAccount", new { hashedAccountId = x.AccountHashId }));
                return Ok(result.Data);
            }
            else
            {
                //TODO: Handle unhappy paths.
                return Conflict();
            }
        }

        [Route("{hashedAccountId}", Name = "GetAccount")]
        [Authorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAccount(string hashedAccountId)
        {
            var result = await _orchestrator.GetAccount(hashedAccountId);

            if (result.Data != null)
            {
                result.Data.LegalEntities.ForEach(x => x.Href = Url.Link("GetLegalEntity", new { hashedAccountId, legalEntityId = x.Id }));
                result.Data.PayeSchemes.ForEach(x => x.Href = Url.Link("GetPayeScheme", new { hashedAccountId, payeSchemeRef = HttpUtility.UrlEncode(x.Id) }));
                return Ok(result.Data);
            }
            else
            {
                return NotFound();
            }
        }

        [Route("{hashedAccountId}/legalentities", Name = "GetLegalEntities")]
        [Authorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLegalEntities(string hashedAccountId)
        {
            return NotFound();
        }

        [Route("{hashedAccountId}/legalentities/{legalentityid}", Name = "GetLegalEntity")]
        [Authorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLegalEntity(string hashedAccountId, long legalEntityId)
        {
            return NotFound();
        }

        [Route("{hashedAccountId}/payeschemes", Name = "GetPayeSchemes")]
        [Authorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPayeSchemes(string hashedAccountId)
        {
            return NotFound();
        }

        [Route("{hashedAccountId}/payeschemes/{payeschemeref}", Name = "GetPayeScheme")]
        [Authorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPayeScheme(string hashedAccountId, string payeSchemeRef)
        {
            return NotFound();
        }
    }
}
