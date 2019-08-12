using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/payeschemes")]
    public class AccountPayeSchemesController : ApiController
    {
        private readonly AccountsOrchestrator _orchestrator;
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;

        public AccountPayeSchemesController(AccountsOrchestrator orchestrator, EmployerApprenticeshipsServiceConfiguration configuration)
        {
            _orchestrator = orchestrator;
            _configuration = configuration;
        }

        [Route("", Name = "GetPayeSchemes")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPayeSchemes(string hashedAccountId)
        {
            return Redirect(_configuration.EmployerAccountsApiBaseUrl + $"/api/accounts/{hashedAccountId}/payeschemes");
        }

        [Route("{payeschemeref}", Name = "GetPayeScheme")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPayeScheme(string hashedAccountId, string payeSchemeRef)
        {
            return Redirect(_configuration.EmployerAccountsApiBaseUrl + $"/api/accounts/{hashedAccountId}/payeschemes/{HttpUtility.UrlEncode(payeSchemeRef)}");
        }
    }
}