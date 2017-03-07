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
                    Severity = FlashMessageSeverityLevel.Success
                };

                TempData.Remove("agreementSigned");
            }

            return View(model);
        }

        [HttpGet]
        [Route("agreements/{agreementId}/details")]
        public async Task<ActionResult> Details(string agreementId, string hashedAccountId, FlashMessageViewModel flashMessage)
        {
            var agreement = await _orchestrator.GetById(agreementId, hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            return View(agreement);
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
        [Route("agreements/{agreementId}/sign")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Sign(string agreementId, string hashedAccountId)
        {
            
            var response = await _orchestrator.SignAgreement(agreementId, hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), DateTime.UtcNow);

            if (response.Status == HttpStatusCode.OK)
            {
                TempData["agreementSigned"] = "true";

                return RedirectToAction("Index", new { hashedAccountId });
            }

            var agreement = await _orchestrator.GetById(agreementId, hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
            agreement.Exception = response.Exception;
            agreement.Status = response.Status;

            return View("SignAgreement", agreement);
        }

        [HttpGet]
        [Route("agreements/{agreementId}/agreement-pdf/{agreementName}")]
        public async Task<FileStreamResult> GetPdfAgreement(string agreementId, string hashedAccountId, string agreementName)
        {

            var stream = await _orchestrator.GetPdfEmployerAgreement(agreementName);

            return new FileStreamResult(stream.Data.PdfStream,"application/pdf");
        }

        [HttpGet]
        [Route("agreements/{agreementId}/signed-agreement-pdf")]
        public async Task<FileStreamResult> GetSignedPdfAgreement(string agreementId, string hashedAccountId)
        {

            var stream = await _orchestrator.GetSignedPdfEmployerAgreement(hashedAccountId,agreementId,OwinWrapper.GetClaimValue("sub"));

            return new FileStreamResult(stream.Data.PdfStream, "application/pdf");
        }
    }
}