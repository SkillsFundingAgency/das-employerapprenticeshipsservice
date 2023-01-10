using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerAccounts.Api.Orchestrators;
using SFA.DAS.EmployerAccounts.Api.Types;

namespace SFA.DAS.EmployerAccounts.Api.Controllers
{
    [Route("api/accounts")]
    public class EmployerAccountsController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        private readonly AccountsOrchestrator _orchestrator;
      
        public EmployerAccountsController(AccountsOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [Route("", Name = "AccountsIndex")]
        [Authorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IActionResult> GetAccounts(string toDate = null, int pageSize = 1000, int pageNumber = 1)
        {
            var result = await _orchestrator.GetAccounts(toDate, pageSize, pageNumber);

            result.Data.ForEach(x => x.Href = Url.RouteUrl("GetAccount", new { hashedAccountId = x.AccountHashId }));
            return Ok(result);
        }

        [Route("{hashedAccountId}", Name = "GetAccount")]
        [Authorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IActionResult> GetAccount(string hashedAccountId)
        {
            var result = await _orchestrator.GetAccount(hashedAccountId);

            if (result == null) return NotFound();
             
            result.LegalEntities.ForEach(x => CreateGetLegalEntityLink(hashedAccountId, x));
            result.PayeSchemes.ForEach(x => CreateGetPayeSchemeLink(hashedAccountId, x));
            return Ok(result);
        }

        [Route("{hashedAccountId}/users", Name = "GetAccountUsers")]
        [Authorize(Roles = "ReadAllAccountUsers")]
        [HttpGet]
        public async Task<IActionResult> GetAccountUsers(string hashedAccountId)
        {
            var result = await _orchestrator.GetAccountTeamMembers(hashedAccountId);
            return Ok(result);
        }

        [Route("internal/{accountId}/users", Name = "GetAccountUsersByInternalAccountId")]
        [Authorize(Roles = "ReadAllAccountUsers")]
        [HttpGet]
        public async Task<IActionResult> GetAccountUsers(long accountId)
        {
            var result = await _orchestrator.GetAccountTeamMembers(accountId);
            return Ok(result);
        }

        [Route("internal/{accountId}/users/which-receive-notifications", Name = "GetAccountUsersByInteralIdWhichReceiveNotifications")]
        [Authorize(Roles = "ReadAllAccountUsers")]
        [HttpGet]
        public async Task<IActionResult> GetAccountUsersWhichReceiveNotifications(long accountId)
        {
            var result = await _orchestrator.GetAccountTeamMembersWhichReceiveNotifications(accountId);
            return Ok(result);
        }

        private void CreateGetLegalEntityLink(string hashedAccountId, Resource legalEntity)
        {
            legalEntity.Href = Url.RouteUrl("GetLegalEntity", new { hashedAccountId, legalEntityId = legalEntity.Id });
        }

        private void CreateGetPayeSchemeLink(string hashedAccountId, Resource payeScheme)
        {
            payeScheme.Href = Url.RouteUrl("GetPayeScheme", new { hashedAccountId, payeSchemeRef = WebUtility.UrlEncode(payeScheme.Id) });
        }
    }
}
