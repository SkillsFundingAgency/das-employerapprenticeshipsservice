using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Web.Helpers;
using SFA.DAS.EAS.Support.Web.Models;
using SFA.DAS.EAS.Support.Web.Services;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Support.Shared.Discovery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.EAS.Support.Web.Controllers
{
    [Authorize(Roles = "das-support-portal")]
    public class AccountController : Controller
    {
        private readonly IAccountHandler _accountHandler;
        private readonly IPayeLevySubmissionsHandler _payeLevySubmissionsHandler;
        private readonly ILogger<AccountController> _log;
        private readonly IPayeLevyMapper _payeLevyMapper;
        private readonly HttpContext _httpContext;

        public AccountController(IAccountHandler accountHandler,
            IPayeLevySubmissionsHandler payeLevySubmissionsHandler,
            ILogger<AccountController> log,
            IPayeLevyMapper payeLevyDeclarationMapper,
            HttpContext httpContext)
        {
            _accountHandler = accountHandler;
            _payeLevySubmissionsHandler = payeLevySubmissionsHandler;
            _log = log;
            _payeLevyMapper = payeLevyDeclarationMapper;
            _httpContext = httpContext;
        }

        [Route("account/{id}")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Index(string id)
        {
            var response = await _accountHandler.FindOrganisations(id);            

            if (response.StatusCode == SearchResponseCodes.Success)
            {
                var vm = new AccountDetailViewModel
                {
                    Account = response.Account,
                    AccountUri = $"/resource/index/{{0}}?key={SupportServiceResourceKey.EmployerUser}"

                };

                return View(vm);
            }
            
            return NotFound();
        }

        [Route("account/payeschemes/{id}")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> PayeSchemes(string id)
        {            
            var response = await _accountHandler.FindPayeSchemes(id);

            if (response.StatusCode == SearchResponseCodes.Success)
            {
                var vm = new AccountDetailViewModel
                {
                    Account = response.Account,
                    AccountUri = $"/resource/index/{{0}}?key={SupportServiceResourceKey.EmployerUser}"
                };

                return View(vm);
            }
            
            return new Microsoft.AspNetCore.Mvc.NotFoundResult();
        }

        [Route("account/header/{id}")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Header(string id)
        {
            var response = await _accountHandler.Find(id);            

            if (response.StatusCode != SearchResponseCodes.Success)
                return NotFound();

            return View("SubHeader", response.Account);
        }

        [Route("account/team/{id}")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Team(string id)
        {
            var response = await _accountHandler.FindTeamMembers(id);

            if (response.StatusCode == SearchResponseCodes.Success)
            {
                var vm = new AccountDetailViewModel
                {
                    Account = response.Account,
                    AccountUri = $"/resource/index/{{0}}?key={SupportServiceResourceKey.EmployerUser}",
                    IsTier2User = _httpContext.User.IsInRole(AuthorizationConstants.Tier2User)
                };

                return View(vm);
            }

            return NotFound();
        }

        [Route("account/finance/{id}")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Finance(string id)
        {
            var response = await _accountHandler.FindFinance(id);

            if (response.StatusCode == SearchResponseCodes.Success)
            {
                var vm = new FinanceViewModel
                {
                    Account = response.Account,
                    Balance = response.Balance
                };

                return View(vm);
            }

            return NotFound();
        }

        [Route("account/levysubmissions/{id}/{payeSchemeId}")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> PayeSchemeLevySubmissions(string id, string payeSchemeId)
        {
            var response = await _payeLevySubmissionsHandler.FindPayeSchemeLevySubmissions(id, payeSchemeId);

            if (response.StatusCode != PayeLevySubmissionsResponseCodes.AccountNotFound)
            {
                var model = _payeLevyMapper.MapPayeLevyDeclaration(response);

                model.UnexpectedError =
                    response.StatusCode == PayeLevySubmissionsResponseCodes.UnexpectedError;

                return View(model);
            }

            return NotFound();
        }
    }
}