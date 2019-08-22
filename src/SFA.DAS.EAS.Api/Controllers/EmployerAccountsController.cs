using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [RoutePrefix("api/accounts")]
    public class EmployerAccountsController : ApiController
    {
        private readonly AccountsOrchestrator _orchestrator;
        private readonly EmployerAccountsApiConfiguration _employerAccountsApiConfiguration;

        public EmployerAccountsController(AccountsOrchestrator orchestrator, EmployerAccountsApiConfiguration employerAccountsApiConfiguration)
        {
            _orchestrator = orchestrator;
            _employerAccountsApiConfiguration = employerAccountsApiConfiguration;
        }

        [Route("", Name = "AccountsIndex")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]   
        public async Task<IHttpActionResult> GetAccounts(string toDate = null, int pageSize = 1000, int pageNumber = 1)
        {
            var result = await _orchestrator.GetAllAccountsWithBalances(toDate, pageSize, pageNumber);
            
            if (result.Status == HttpStatusCode.OK)
            {
                result.Data.Data.ForEach(x => x.Href = Url.Route("GetAccount", new { hashedAccountId = x.AccountHashId }));
                return Ok(result.Data);
            }
        
            return Conflict();
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
         
            return Ok(result.Data);
        }

        [Route("{hashedAccountId}/users", Name = "GetAccountUsers")]
        [ApiAuthorize(Roles = "ReadAllAccountUsers")]
        [HttpGet]
        public IHttpActionResult GetAccountUsers(string hashedAccountId)
        {
            return Redirect($"{_employerAccountsApiConfiguration.BaseUrl}/api/accounts/{hashedAccountId}/users");
        }

        [Route("internal/{accountId}/users", Name = "GetAccountUsersByInternalAccountId")]
        [ApiAuthorize(Roles = "ReadAllAccountUsers")]
        [HttpGet]
        public IHttpActionResult GetAccountUsers(long accountId)
        {
            return Redirect($"{_employerAccountsApiConfiguration.BaseUrl}/api/accounts/internal/{accountId}/users");
        }
    }
}
