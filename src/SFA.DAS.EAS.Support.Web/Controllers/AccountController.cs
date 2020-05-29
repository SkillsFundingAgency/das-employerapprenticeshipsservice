using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Web.Helpers;
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
        private readonly HttpContextBase _httpContext;

        public AccountController(IAccountHandler accountHandler,
            IPayeLevySubmissionsHandler payeLevySubmissionsHandler,
            ILog log,
            IPayeLevyMapper payeLevyDeclarationMapper,
            HttpContextBase httpContext)
        {
            _accountHandler = accountHandler;
            _payeLevySubmissionsHandler = payeLevySubmissionsHandler;
            _log = log;
            _payeLevyMapper = payeLevyDeclarationMapper;
            _httpContext = httpContext;
        }

        [Route("account/{id}")]
        public async Task<ActionResult> Index(string id)
        {
            _log.Info($"AccountController-Index : Getting FindOrganisations Response for id : {id}");

            var response = await _accountHandler.FindOrganisations(id);

            _log.Info($"AccountController-Index : FindOrganisations Response : {response}"); 

            if (response.StatusCode == SearchResponseCodes.Success)
            {
                var vm = new AccountDetailViewModel
                {
                    Account = response.Account,
                    AccountUri = $"/resource/index/{{0}}?key={SupportServiceResourceKey.EmployerUser}"

                };

                return View(vm);
            }

            _log.Info($"AccountController-Index : HttpNotFound for id : {id} ");
            return HttpNotFound();
        }

        [Route("account/payeschemes/{id}")]
        public async Task<ActionResult> PayeSchemes(string id)
        {
            _log.Info($"AccountController-PayeSchemes : Getting FindPayeSchemes Response for id : {id}");
            
            var response = await _accountHandler.FindPayeSchemes(id);

            _log.Info($"AccountController-PayeSchemes : FindPayeSchemes Response : {response}");

            if (response.StatusCode == SearchResponseCodes.Success)
            {
                var vm = new AccountDetailViewModel
                {
                    Account = response.Account,
                    AccountUri = $"/resource/index/{{0}}?key={SupportServiceResourceKey.EmployerUser}"
                };

                return View(vm);
            }

            _log.Info($"AccountController-PayeSchemes : HttpNotFound for id : {id} ");
            return new HttpNotFoundResult();
        }

        [Route("account/header/{id}")]
        public async Task<ActionResult> Header(string id)
        {
            _log.Info($"AccountController-Header : Find Header by id : {id}");

            var response = await _accountHandler.Find(id);

            _log.Info($"AccountController-Header : Header Response : {response}");

            if (response.StatusCode != SearchResponseCodes.Success)
                return HttpNotFound();

            _log.Info($"AccountController-Header : HttpNotFound for id : {id} ");

            return View("SubHeader", response.Account);
        }

        [Route("account/team/{id}")]
        public async Task<ActionResult> Team(string id)
        {
            _log.Info($"AccountController-Team : Getting FindTeamMembers Response for id : {id}");

            var response = await _accountHandler.FindTeamMembers(id);

            _log.Info($"AccountController-Team : FindTeamMembers Response : {response}");

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

            _log.Info($"AccountController-Team : HttpNotFound for id : {id} ");

            return HttpNotFound();
        }

        [Route("account/finance/{id}")]
        public async Task<ActionResult> Finance(string id)
        {
            _log.Info($"AccountController-Finance : Find Finance by id : {id}");

            var response = await _accountHandler.FindFinance(id);

            _log.Info($"AccountController-Finance : FindFinance Response : {response}");

            if (response.StatusCode == SearchResponseCodes.Success)
            {
                var vm = new FinanceViewModel
                {
                    Account = response.Account,
                    Balance = response.Balance
                };

                return View(vm);
            }

            _log.Info($"AccountController-Finance : HttpNotFound for id : {id} ");

            return HttpNotFound();
        }

        [Route("account/levysubmissions/{id}/{payeSchemeId}")]
        public async Task<ActionResult> PayeSchemeLevySubmissions(string id, string payeSchemeId)
        {
            _log.Info($"AccountController-PayeSchemeLevySubmissions : Getting PayeLevySubmissionsResponseCodes Response for id : {id}");

            var response = await _payeLevySubmissionsHandler.FindPayeSchemeLevySubmissions(id, payeSchemeId);

            _log.Info($"AccountController-PayeSchemeLevySubmissions : PayeLevySubmissionsResponseCodes Response : {response}");

            if (response.StatusCode != PayeLevySubmissionsResponseCodes.AccountNotFound)
            {
                var model = _payeLevyMapper.MapPayeLevyDeclaration(response);

                model.UnexpectedError =
                    response.StatusCode == PayeLevySubmissionsResponseCodes.UnexpectedError;

                return View(model);
            }

            _log.Info($"AccountController-PayeLevySubmissionsResponseCodes : HttpNotFound for id : {id} ");

            return HttpNotFound();
        }
    }
}