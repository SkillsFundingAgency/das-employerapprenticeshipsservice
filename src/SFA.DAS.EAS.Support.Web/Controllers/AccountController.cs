using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Web.Models;
using SFA.DAS.EAS.Support.Web.Services;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Support.Shared.Discovery;

namespace SFA.DAS.EAS.Support.Web.Controllers
{
    [Authorize(Roles = "das-support-portal")]
    public class AccountController : Controller
    {
        private readonly IAccountHandler _accountHandler;
        private readonly IPayeLevySubmissionsHandler _payeLevySubmissionsHandler;
        private readonly ILog _log;
        private readonly IPayeLevyMapper _payeLevyMapper;

        public AccountController(IAccountHandler accountHandler,
            IPayeLevySubmissionsHandler payeLevySubmissionsHandler,
            ILog log,
            IPayeLevyMapper payeLevyDeclarationMapper)
        {
            _accountHandler = accountHandler;
            _payeLevySubmissionsHandler = payeLevySubmissionsHandler;
            _log = log;
            _payeLevyMapper = payeLevyDeclarationMapper;
        }

        [Route("account/{id}")]
        public async Task<ActionResult> Index(string id)
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

            return HttpNotFound();
        }

        [Route("account/payeschemes/{id}")]
        public async Task<ActionResult> PayeSchemes(string id)
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

            return new HttpNotFoundResult();
        }

        [Route("account/header/{id}")]
        public async Task<ActionResult> Header(string id)
        {
            var response = await _accountHandler.Find(id);

            if (response.StatusCode != SearchResponseCodes.Success)
                return HttpNotFound();

            return View("SubHeader", response.Account);
        }

        [Route("account/team/{id}")]
        public async Task<ActionResult> Team(string id)
        {
            var response = await _accountHandler.FindTeamMembers(id);

            if (response.StatusCode == SearchResponseCodes.Success)
            {
                var vm = new AccountDetailViewModel
                {
                    Account = response.Account,
                    AccountUri = $"/resource/index/{{0}}?key={SupportServiceResourceKey.EmployerUser}"
                };

                return View(vm);
            }

            return HttpNotFound();
        }

        [Route("account/finance/{id}")]
        public async Task<ActionResult> Finance(string id)
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

            return HttpNotFound();
        }

        [Route("account/levysubmissions/{id}/{payeSchemeId}")]
        public async Task<ActionResult> PayeSchemeLevySubmissions(string id, string payeSchemeId)
        {
            var response = await _payeLevySubmissionsHandler.FindPayeSchemeLevySubmissions(id, payeSchemeId);

            if (response.StatusCode != PayeLevySubmissionsResponseCodes.AccountNotFound)
            {
                var model = _payeLevyMapper.MapPayeLevyDeclaration(response);

                model.UnexpectedError =
                    response.StatusCode == PayeLevySubmissionsResponseCodes.UnexpectedError;

                return View(model);
            }

            return HttpNotFound();
        }
    }
}