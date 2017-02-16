using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{HashedAccountId}")]
    public class EmployerAgreementController : BaseController
    {
        private readonly EmployerAgreementOrchestrator _orchestrator;

        public EmployerAgreementController(IOwinWrapper owinWrapper, EmployerAgreementOrchestrator orchestrator, 
            IFeatureToggle featureToggle, IUserWhiteList userWhiteList) 
            : base(owinWrapper, featureToggle, userWhiteList)
        {
            if (owinWrapper == null)
                throw new ArgumentNullException(nameof(owinWrapper));
            if (orchestrator == null)
                throw new ArgumentNullException(nameof(orchestrator));
          
            _orchestrator = orchestrator;
        }

        [HttpGet]
        [Route("agreements")]
        public async Task<ActionResult> Index(string hashedAccountId, FlashMessageViewModel flashMessage)
        {
            var model = await _orchestrator.Get(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            if (TempData.ContainsKey("agreementSigned"))
            {
                model.FlashMessage = new FlashMessageViewModel
                {
                    Headline = "Agreement signed",
                    Message = $"You've signed the agreement for {TempData["agreementSigned"]}",
                    Severity = FlashMessageSeverityLevel.Success
                };
            }

            return View(model);
        }
        
		[HttpGet]
		[Route("agreements/{agreementId}/view")]
        public async Task<ActionResult> View(string agreementId, string hashedAccountId, FlashMessageViewModel flashMessage)
        {
            var agreement = await _orchestrator.GetById(agreementId, hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
            
            return View(agreement);
        }

        [HttpGet]
        [Route("agreements/{agreementId}/about-your-agreement")]
        public async Task<ActionResult> AboutYourAgreement(string agreementid, string hashedAccountId)
        {
            var agreement = await _orchestrator.GetById(agreementid, hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            return View(agreement);
        }

        [HttpGet]
        [Route("agreements/{agreementId}/sign-your-agreement")]
        public async Task<ActionResult> SignAgreement(string agreementId, string hashedAccountId)
        {
            var agreement = await _orchestrator.GetById(agreementId, hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            return View(agreement);
        }

        [HttpPost]
        [Route("agreements/{agreementid}/sign")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Sign(string agreementid, string hashedAccountId, string understood, string legalEntityName)
        {
            if (understood == nameof(understood))
            {
                var response = await _orchestrator.SignAgreement(agreementid, hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), DateTime.Now);

                if (response.Status == HttpStatusCode.OK)
                {
                    TempData["agreementSigned"] = legalEntityName;

                    return RedirectToAction("Index", new { hashedAccountId });
                }
            }

            TempData["notunderstood"] = true;

            return RedirectToAction("SignAgreement", new { agreementId = agreementid, hashedAccountId });
        }

    }
}