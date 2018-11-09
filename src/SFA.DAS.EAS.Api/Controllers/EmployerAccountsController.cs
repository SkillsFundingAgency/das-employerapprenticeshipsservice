using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Account.Api.Extensions;
using SFA.DAS.EAS.Account.Api.Orchestrators;

namespace SFA.DAS.EAS.Account.Api.Controllers
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
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]   
        public IHttpActionResult GetAccounts(string toDate = null, int pageSize = 1000, int pageNumber = 1)
        {
            return Redirect(Url.EmployerAccountsApiAction(Request.RequestUri.PathAndQuery));
        }

        [Route("2", Name = "AccountsIndex2")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAccounts2(string toDate = null, int pageSize = 1000, int pageNumber = 1)
        {
            var result = await _orchestrator.GetAllAccountsWithBalances(toDate, pageSize, pageNumber);

            if (result.Status == HttpStatusCode.OK)
            {
                result.Data.Data.ForEach(x => x.Href = Url.Route("GetAccount", new { hashedAccountId = x.AccountHashId }));
                return Ok(result.Data);
            }
            else
            {
                //TODO: Handle unhappy paths.
                return Conflict();
            }
        }

        [Route("{hashedAccountId}", Name = "GetAccount")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public IHttpActionResult GetAccount(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsApiAction(Request.RequestUri.PathAndQuery));
        }

        [Route("{hashedAccountId}/2", Name = "GetAccount2")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAccount2(string hashedAccountId)
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

        [Route("internal/{accountId}", Name = "GetAccountByInternalId")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAccount(long accountId)
        {
            var result = await _orchestrator.GetAccount(accountId);

            if (result.Data == null)
            {
                return NotFound();
            }

            result.Data.LegalEntities.ForEach(x => CreateGetLegalEntityLink(result.Data.HashedAccountId, x));
            result.Data.PayeSchemes.ForEach(x => CreateGetPayeSchemeLink(result.Data.HashedAccountId, x));
            return Ok(result.Data);
        }



        [Route("{hashedAccountId}/users", Name = "GetAccountUsers")]
        [ApiAuthorize(Roles = "ReadAllAccountUsers")]
        [HttpGet]
        public IHttpActionResult GetAccountUsers(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsApiAction(Request.RequestUri.PathAndQuery));
        }

        [Route("{hashedAccountId}/users2", Name = "GetAccountUsers2")]
        [ApiAuthorize(Roles = "ReadAllAccountUsers")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAccountUsers2(string hashedAccountId)
        {
            var result = await _orchestrator.GetAccountTeamMembers(hashedAccountId);

            if (result.Data == null)
            {
                return NotFound();
            }

            return Ok(result.Data);
        }

        [Route("internal/{accountId}/users", Name = "GetAccountUsersByInternalAccountId")]
        [ApiAuthorize(Roles = "ReadAllAccountUsers")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAccountUsers(long accountId)
        {
            var result = await _orchestrator.GetAccountTeamMembers(accountId);

            if (result.Data == null)
            {
                return NotFound();
            }

            return Ok(result.Data);
        }
        

        private void CreateGetLegalEntityLink(string hashedAccountId, ResourceViewModel legalEntity)
        {
            legalEntity.Href = Url.Route("GetLegalEntity", new { hashedAccountId, legalEntityId = legalEntity.Id });
        }

        private void CreateGetPayeSchemeLink(string hashedAccountId, ResourceViewModel payeScheme)
        {
            payeScheme.Href = Url.Route("GetPayeScheme", new { hashedAccountId, payeSchemeRef = HttpUtility.UrlEncode(payeScheme.Id) });
        }
    }
}
