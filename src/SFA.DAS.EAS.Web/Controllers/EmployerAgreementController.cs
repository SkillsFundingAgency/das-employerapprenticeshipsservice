using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Infrastructure.Authentication;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Helpers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Organisation;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{HashedAccountId}")]
    public class EmployerAgreementController : BaseController
    {
        private readonly EmployerAgreementOrchestrator _orchestrator;

        public EmployerAgreementController(IAuthenticationService owinWrapper, EmployerAgreementOrchestrator orchestrator, 
            IAuthorizationService authorization, IMultiVariantTestingService multiVariantTestingService, 
            ICookieStorageService<FlashMessageViewModel> flashMessage) 
            : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
            if (owinWrapper == null)
                throw new ArgumentNullException(nameof(owinWrapper));
            if (orchestrator == null)
                throw new ArgumentNullException(nameof(orchestrator));
          
            _orchestrator = orchestrator;
        }

        [HttpGet]
        [Route("agreements")]
        public async Task<ActionResult> Index(string hashedAccountId)
        {
            var model = await _orchestrator.Get(hashedAccountId, OwinWrapper.GetClaimValue(ControllerConstants.UserExternalIdClaimKeyName));

            var flashMessage = GetFlashMessageViewModelFromCookie();
            if (flashMessage!=null)
            {
                model.FlashMessage = flashMessage;
            }

            return View(model);
        }

        [HttpGet]
        [Route("agreements/{agreementId}/details")]
        public async Task<ActionResult> Details(string agreementId, string hashedAccountId, FlashMessageViewModel flashMessage)
        {
            var agreement = await _orchestrator.GetById(agreementId, hashedAccountId, OwinWrapper.GetClaimValue(ControllerConstants.UserExternalIdClaimKeyName));

            return View(agreement);
        }

        [HttpGet]
		[Route("agreements/{agreementId}/view")]
        public async Task<ActionResult> View(string agreementId, string hashedAccountId, FlashMessageViewModel flashMessage)
        {
            var agreement = await _orchestrator.GetById(agreementId, hashedAccountId, OwinWrapper.GetClaimValue(ControllerConstants.UserExternalIdClaimKeyName));
            
            return View(agreement);
        }

        [HttpGet]
        [Route("agreements/unsigned/view")]
        public async Task<ActionResult> ViewUnsignedAgreements(string hashedAccountId)
        {
            var agreements = await _orchestrator.Get(hashedAccountId, OwinWrapper.GetClaimValue(ControllerConstants.UserExternalIdClaimKeyName));

            var unsignedAgreements = agreements.Data
                                        .EmployerAgreements
                                        .Where(x => x.HasPendingAgreement)
                                        .Take(2)
                                        .ToArray();

            if (unsignedAgreements?.Length != 1)
                return RedirectToAction("Index");

            var hashedAgreementId = unsignedAgreements[0].Pending.HashedAgreementId;

            return RedirectToAction("AboutYourAgreement", new { agreementId = hashedAgreementId });
        }

        [HttpGet]
        [Route("agreements/{agreementId}/about-your-agreement")]
        public async Task<ActionResult> AboutYourAgreement(string agreementid, string hashedAccountId)
        {
            var agreement = await _orchestrator.GetById(agreementid, hashedAccountId, OwinWrapper.GetClaimValue(ControllerConstants.UserExternalIdClaimKeyName));

            return View(agreement);
        }

        [HttpGet]
        [Route("agreements/{agreementId}/sign-your-agreement")]
        public async Task<ActionResult> SignAgreement(string agreementId, string hashedAccountId)
        {
            var agreement = await _orchestrator.GetById(agreementId, hashedAccountId, OwinWrapper.GetClaimValue(ControllerConstants.UserExternalIdClaimKeyName));

            return View(agreement);
        }

        [HttpPost]
        [Route("agreements/{agreementId}/sign")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Sign(string agreementId, string hashedAccountId)
        {
            var userInfo = OwinWrapper.GetClaimValue(ControllerConstants.UserExternalIdClaimKeyName);
            var agreement = await _orchestrator.GetById(agreementId, hashedAccountId, userInfo);
            var response = await _orchestrator.SignAgreement(agreementId, hashedAccountId, userInfo, DateTime.UtcNow, agreement.Data.EmployerAgreement.LegalEntityName);

            if (response.Status == HttpStatusCode.OK)
            {
                var flashMessage = new FlashMessageViewModel
                {
                    Headline = "Agreement signed",
                    Severity = FlashMessageSeverityLevel.Success
                };
                AddFlashMessageToCookie(flashMessage);

                return RedirectToAction(ControllerConstants.NextStepsActionName, new { hashedAccountId });
            }

            
            agreement.Exception = response.Exception;
            agreement.Status = response.Status;

            return View(ControllerConstants.SignAgreementViewName, agreement);
        }

        [HttpGet]
        [Route("agreements/{agreementId}/next")]
        public async Task<ActionResult> NextSteps(string hashedAccountId)
        {
            var userId = OwinWrapper.GetClaimValue(ControllerConstants.UserExternalIdClaimKeyName);

            var userShownWizard = await _orchestrator.UserShownWizard(userId, hashedAccountId);

            var model = new OrchestratorResponse<EmployerAgreementNextStepsViewModel>
            {
                FlashMessage = GetFlashMessageViewModelFromCookie(), Data = new EmployerAgreementNextStepsViewModel { UserShownWizard = userShownWizard}
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("agreements/{agreementId}/next")]
        public async Task<ActionResult> NextSteps(int? choice, string hashedAccountId)
        {
            var userId = OwinWrapper.GetClaimValue(ControllerConstants.UserExternalIdClaimKeyName);

            var userShownWizard = await _orchestrator.UserShownWizard(userId, hashedAccountId);

            switch (choice ?? 0)
            {
                case 1: return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.EmployerAgreementControllerName);
                case 2: return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.EmployerAccountTransactionsControllerName);
                case 3: return RedirectToAction(ControllerConstants.InformActionName, ControllerConstants.EmployerCommitmentsControllerName);
                case 4: return RedirectToAction(ControllerConstants.IndexActionName,   ControllerConstants.EmployerTeamControllerName);
                default:
                    var model = new OrchestratorResponse<EmployerAgreementNextStepsViewModel>
                    {
                        FlashMessage = GetFlashMessageViewModelFromCookie(),
                        Data = new EmployerAgreementNextStepsViewModel
                        {
                            ErrorMessage = "You must select an option to continue.",
                            UserShownWizard = userShownWizard
                        }
                    };
                    return View(model); //No option entered
            }
        }

        [HttpGet]
        [Route("agreements/{agreementId}/agreement-pdf")]
        public async Task<ActionResult> GetPdfAgreement(string agreementId, string hashedAccountId)
        {

            var stream = await _orchestrator.GetPdfEmployerAgreement(hashedAccountId,agreementId, OwinWrapper.GetClaimValue(ControllerConstants.UserExternalIdClaimKeyName));

            if (stream.Data.PdfStream == null)
            {
                // ReSharper disable once Mvc.ViewNotResolved
                return View(stream);
            }

            return new FileStreamResult(stream.Data.PdfStream, ControllerConstants.PdfContentTypeName);
        }

        [HttpGet]
        [Route("agreements/{agreementId}/signed-agreement-pdf")]
        public async Task<ActionResult> GetSignedPdfAgreement(string agreementId, string hashedAccountId)
        {

            var stream = await _orchestrator.GetSignedPdfEmployerAgreement(hashedAccountId,agreementId,OwinWrapper.GetClaimValue(ControllerConstants.UserExternalIdClaimKeyName));

            if (stream.Data.PdfStream == null)
            {
                // ReSharper disable once Mvc.ViewNotResolved
                return View(stream);
            }

            return new FileStreamResult(stream.Data.PdfStream, ControllerConstants.PdfContentTypeName);
        }

        [HttpGet]
        [Route("agreements/remove")]
        public async Task<ActionResult> GetOrganisationsToRemove(string hashedAccountId)
        {
            var model = await _orchestrator.GetLegalAgreementsToRemove(hashedAccountId, OwinWrapper.GetClaimValue(ControllerConstants.UserExternalIdClaimKeyName));

            return View(model);
        }

        [HttpGet]
        [Route("agreements/remove/{agreementId}")]
        public async Task<ActionResult> ConfirmRemoveOrganisation(string agreementId, string hashedAccountId)
        {
            var model = await _orchestrator.GetConfirmRemoveOrganisationViewModel(agreementId, hashedAccountId, OwinWrapper.GetClaimValue(ControllerConstants.UserExternalIdClaimKeyName));

            var flashMessage = GetFlashMessageViewModelFromCookie();
            if (flashMessage != null)
            {
                model.FlashMessage = flashMessage;
                model.Data.ErrorDictionary = model.FlashMessage.ErrorMessages;
            }

            return View(model);
        }

        [HttpPost]
        [Route("agreements/remove/{agreementId}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveOrganisation(string hashedAccountId, string agreementId, ConfirmLegalAgreementToRemoveViewModel model)
        {
            var response = await _orchestrator.RemoveLegalAgreement(model, OwinWrapper.GetClaimValue(ControllerConstants.UserExternalIdClaimKeyName));

            if (response.Status == HttpStatusCode.OK)
            {
                AddFlashMessageToCookie(response.FlashMessage);

                return RedirectToAction(ControllerConstants.IndexActionName, new { hashedAccountId });
            }
            if (response.Status == HttpStatusCode.BadRequest)
            {
                AddFlashMessageToCookie(response.FlashMessage);
                return RedirectToAction(ControllerConstants.ConfirmRemoveOrganisationActionName, new {hashedAccountId, agreementId});
            }

            return RedirectToAction(ControllerConstants.IndexActionName, new { hashedAccountId });
        }

    }
}