using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Types;
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

            if (result.Data == null)
            {
                return NotFound();
            }

            result.Data.LegalEntities.ForEach(x => CreateGetLegalEntityLink(hashedAccountId, x));
            result.Data.PayeSchemes.ForEach(x => CreateGetPayeSchemeLink(hashedAccountId, x));
            return Ok(result.Data);
        }

        [Route("{hashedAccountId}/legalentities", Name = "GetLegalEntities")]
        [Authorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLegalEntities(string hashedAccountId)
        {
            var result = await _orchestrator.GetAccount(hashedAccountId);

            if (result.Data == null)
            {
                return NotFound();
            }

            result.Data.LegalEntities.ForEach(x => CreateGetLegalEntityLink(hashedAccountId, x));
            return Ok(result.Data.LegalEntities);
        }

        [Route("{hashedAccountId}/legalentities/{legalentityid}", Name = "GetLegalEntity")]
        [Authorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLegalEntity(string hashedAccountId, long legalEntityId)
        {
            var result = await _orchestrator.GetLegalEntity(legalEntityId);

            if (result.Data == null)
            {
                return NotFound();
            }

            return Ok(result.Data);
        }

        [Route("{hashedAccountId}/payeschemes", Name = "GetPayeSchemes")]
        [Authorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPayeSchemes(string hashedAccountId)
        {
            var result = await _orchestrator.GetAccount(hashedAccountId);

            if (result.Data == null)
            {
                return NotFound();
            }

            result.Data.PayeSchemes.ForEach(x => CreateGetPayeSchemeLink(hashedAccountId, x));
            return Ok(result.Data.PayeSchemes);
        }

        [Route("{hashedAccountId}/payeschemes/{payeschemeref}", Name = "GetPayeScheme")]
        [Authorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPayeScheme(string hashedAccountId, string payeSchemeRef)
        {
            return NotFound();
        }

        private void CreateGetLegalEntityLink(string hashedAccountId, ResourceViewModel legalEntity)
        {
            legalEntity.Href = Url.Link("GetLegalEntity", new { hashedAccountId, legalEntityId = legalEntity.Id });
        }

        private void CreateGetPayeSchemeLink(string hashedAccountId, ResourceViewModel payeScheme)
        {
            payeScheme.Href = Url.Link("GetPayeScheme", new { hashedAccountId, payeSchemeRef = HttpUtility.UrlEncode(payeScheme.Id) });
        }
    }
}
