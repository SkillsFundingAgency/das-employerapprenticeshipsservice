using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

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
        public async Task<ActionResult> Index(long accountId, FlashMessageViewModel flashMessage)
        {
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");
            if (string.IsNullOrWhiteSpace(userIdClaim)) return RedirectToAction("Index", "Home");

            var model = await _orchestrator.Get(accountId, userIdClaim);

            //DANGER: Injected flash messages override the orchestrator response
            if (flashMessage != null) model.FlashMessage = flashMessage;

            return View(model);
        }

        public ActionResult Add()
        {
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");
            if (string.IsNullOrWhiteSpace(userIdClaim)) return RedirectToAction("Index", "Home");

            return View();
        }

        public async Task<ActionResult> View(long agreementid, long accountId, FlashMessageViewModel flashMessage)
        {
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");
            if (string.IsNullOrWhiteSpace(userIdClaim)) return RedirectToAction("Index", "Home");

            var agreement = await _orchestrator.GetById(agreementid, accountId, userIdClaim);
            
            //DANGER: Injected flash messages override the orchestrator response
            if (flashMessage != null) agreement.FlashMessage = flashMessage;

            return View(agreement);
        }

        [HttpPost]
        public async Task<ActionResult> Sign(long agreementid, long accountId, string understood, string legalEntityName)
        {
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");
            if (string.IsNullOrWhiteSpace(userIdClaim)) return RedirectToAction("Index", "Home");

            if (understood == "understood")
            {
                var response = await _orchestrator.SignAgreement(agreementid, accountId, userIdClaim, DateTime.Now);

                if (response.Status == HttpStatusCode.OK)
                {
                    var successMessage = new FlashMessageViewModel()
                    {
                        Headline = "Agreement signed",
                        Message = $"You've signed the agreement for {legalEntityName}",
                        Severity = FlashMessageSeverityLevel.Success
                    };

                    return RedirectToAction("Index", new {accountId = accountId, flashMessage = successMessage });
                }

                return View("DeadView", response);
            }

            TempData["notunderstood"] = new object();
            var errorMessage = new FlashMessageViewModel()
            {
                Message = "You must indicate that you have read and understood the terms",
                Severity = FlashMessageSeverityLevel.Danger
            };          
            return RedirectToAction("View", new { agreementId = agreementid, accountId = accountId, flashMessage = errorMessage });
        }
    }
}