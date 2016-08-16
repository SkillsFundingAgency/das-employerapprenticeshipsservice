using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    public class EmployerAgreementController : BaseController
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

        public async Task<ActionResult> View(long agreementid, long accountId)
        {
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");
            if (string.IsNullOrWhiteSpace(userIdClaim)) return RedirectToAction("Index", "Home");

            var agreement = await _orchestrator.GetById(agreementid, accountId, userIdClaim);

            return View(agreement);
        }

        [HttpPost]
        public async Task<ActionResult> Sign(long agreementid, long accountId, string understood)
        {
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");
            if (string.IsNullOrWhiteSpace(userIdClaim)) return RedirectToAction("Index", "Home");

            if (understood == "understood")
            {
                var response = await _orchestrator.SignAgreement(agreementid, accountId, userIdClaim);

                if (response.Status == HttpStatusCode.OK)
                {
                    TempData["successMessage"] = response.FlashMessage.Message;

                    return RedirectToAction("Index", new {accountId = accountId});
                }

                return View("DeadView", response);
            }

            TempData["successMessage"] = "You must indicate that you have read and understood the terms";

            return RedirectToAction("View", new { agreementId = agreementid, accountId = accountId });
        }
    }
}