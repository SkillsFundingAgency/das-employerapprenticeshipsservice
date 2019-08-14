using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [RoutePrefix("api/accounts")]
    public class EmployerAccountsController : ApiController
    {
        private readonly AccountsOrchestrator _orchestrator;
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;

        public EmployerAccountsController(AccountsOrchestrator orchestrator, EmployerApprenticeshipsServiceConfiguration configuration)
        {
            _orchestrator = orchestrator;
            _configuration = configuration;
        }

        [Route("", Name = "AccountsIndex")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]   
        public IHttpActionResult GetAccounts(string toDate = null, int pageSize = 1000, int pageNumber = 1)
        {
            return Redirect(_configuration.EmployerAccountsApiBaseUrl + $"/api/accounts?pagesize={pageSize}&pagenumber={pageNumber}{(string.IsNullOrWhiteSpace(toDate) ? "" : "&todate=" + toDate)}");
        }

        [Route("{hashedAccountId}", Name = "GetAccount")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
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
        public async Task<IHttpActionResult> GetAccountUsers(string hashedAccountId)
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
