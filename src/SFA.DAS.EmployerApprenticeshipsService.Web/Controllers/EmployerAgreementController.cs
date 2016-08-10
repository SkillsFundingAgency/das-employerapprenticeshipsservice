using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    public class EmployerAgreementController : Controller
    {
        private readonly IOwinWrapper _owinWrapper;
        private readonly EmployerAgreementOrchestrator _orchestrator;

        public EmployerAgreementController(IOwinWrapper owinWrapper, EmployerAgreementOrchestrator orchestrator)
        {
            if (owinWrapper == null)
                throw new ArgumentNullException(nameof(owinWrapper));
            if (orchestrator == null)
                throw new ArgumentNullException(nameof(orchestrator));
            _owinWrapper = owinWrapper;
            _orchestrator = orchestrator;
        }

        [HttpGet]
        public async Task<ActionResult> Index(long accountId)
        {
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");
            if (string.IsNullOrWhiteSpace(userIdClaim)) return RedirectToAction("Index", "Home");

            var model = await _orchestrator.Get(accountId, userIdClaim);

            return View(model);
        }

        public ActionResult Add()
        {
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");
            if (string.IsNullOrWhiteSpace(userIdClaim)) return RedirectToAction("Index", "Home");

            return View();
        }
    }
}