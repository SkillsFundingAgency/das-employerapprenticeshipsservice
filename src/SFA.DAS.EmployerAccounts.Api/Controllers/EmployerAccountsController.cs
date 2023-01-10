using SFA.DAS.EmployerAccounts.Api.Attributes;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SFA.DAS.EmployerAccounts.Api.Orchestrators;
using SFA.DAS.EmployerAccounts.Api.Types;

namespace SFA.DAS.EmployerAccounts.Api.Controllers
{
    [RoutePrefix("api/accounts")]
    public class EmployerAccountsController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        private readonly AccountsOrchestrator _orchestrator;
      
        public EmployerAccountsController(AccountsOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [Route("", Name = "AccountsIndex")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAccounts(string toDate = null, int pageSize = 1000, int pageNumber = 1)
        {
            var result = await _orchestrator.GetAccounts(toDate, pageSize, pageNumber);

            result.Data.ForEach(x => x.Href = Url.Route("GetAccount", new { hashedAccountId = x.AccountHashId }));
            return Ok(result);
        }

        [Route("{hashedAccountId}", Name = "GetAccount")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAccount(string hashedAccountId)
        {
            var result = await _orchestrator.GetAccount(hashedAccountId);

            if (result == null) return NotFound();
             
            result.LegalEntities.ForEach(x => CreateGetLegalEntityLink(hashedAccountId, x));
            result.PayeSchemes.ForEach(x => CreateGetPayeSchemeLink(hashedAccountId, x));
            return Ok(result);
        }

        [Route("{hashedAccountId}/users", Name = "GetAccountUsers")]
        [ApiAuthorize(Roles = "ReadAllAccountUsers")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAccountUsers(string hashedAccountId)
        {
            var result = await _orchestrator.GetAccountTeamMembers(hashedAccountId);
            return Ok(result);
        }

        [Route("internal/{accountId}/users", Name = "GetAccountUsersByInternalAccountId")]
        [ApiAuthorize(Roles = "ReadAllAccountUsers")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAccountUsers(long accountId)
        {
            var result = await _orchestrator.GetAccountTeamMembers(accountId);
            return Ok(result);
        }

        [Route("internal/{accountId}/users/which-receive-notifications", Name = "GetAccountUsersByInteralIdWhichReceiveNotifications")]
        [ApiAuthorize(Roles = "ReadAllAccountUsers")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAccountUsersWhichReceiveNotifications(long accountId)
        {
            var result = await _orchestrator.GetAccountTeamMembersWhichReceiveNotifications(accountId);
            return Ok(result);
        }

        private void CreateGetLegalEntityLink(string hashedAccountId, Resource legalEntity)
        {
            legalEntity.Href = Url.Route("GetLegalEntity", new { hashedAccountId, legalEntityId = legalEntity.Id });
        }

        private void CreateGetPayeSchemeLink(string hashedAccountId, Resource payeScheme)
        {
            payeScheme.Href = Url.Route("GetPayeScheme", new { hashedAccountId, payeSchemeRef = HttpUtility.UrlEncode(payeScheme.Id) });
        }
    }
}
