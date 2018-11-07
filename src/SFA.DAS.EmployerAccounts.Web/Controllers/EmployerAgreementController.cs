using System;
using System.Net;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreement;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{HashedAccountId}")]
    public class EmployerAgreementController : BaseController
    {
        private readonly EmployerAgreementOrchestrator _orchestrator;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;



        public EmployerAgreementController(IAuthenticationService owinWrapper,
            EmployerAgreementOrchestrator orchestrator,
            IAuthorizationService authorization,
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage,
            IMediator mediator,
            IMapper mapper)
            : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
            if (owinWrapper == null)
                throw new ArgumentNullException(nameof(owinWrapper));
            if (orchestrator == null)
                throw new ArgumentNullException(nameof(orchestrator));

            _orchestrator = orchestrator;
            _mediator = mediator;
            _mapper = mapper;
        }

        public EmployerAgreementController(IAuthenticationService owinWrapper) : base(owinWrapper)
        {
        }


        [HttpGet]
        [Route("agreements")]
        public async Task<ActionResult> Index(string hashedAccountId, bool agreementSigned = false)
        {

            var model = await _orchestrator.Get(hashedAccountId,
                OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            var flashMessage = GetFlashMessageViewModelFromCookie();
            if (flashMessage != null)
            {
                model.FlashMessage = flashMessage;
            }

            ViewBag.ShowConfirmation = agreementSigned && model.Data.EmployerAgreementsData.HasPendingAgreements;

            return View(model);

        }

        [HttpGet]
        [Route("agreements/{agreementId}/details")]
        public async Task<ActionResult> Details(string agreementId, string hashedAccountId,
            FlashMessageViewModel flashMessage)
        {
            var agreement = await _orchestrator.GetById(agreementId, hashedAccountId,
                OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));
            return View(agreement);
        }

        [HttpGet]
        [Route("agreements/{agreementId}/view")]
        public async Task<ActionResult> View(string agreementId, string hashedAccountId,
            FlashMessageViewModel flashMessage)
        {
            var agreement = await _orchestrator.GetById(agreementId, hashedAccountId,
                OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));
            return View(agreement);
        }

        [HttpGet]
        [Route("agreements/unsigned/view")]
        public async Task<ActionResult> ViewUnsignedAgreements(string hashedAccountId)
        {
            var agreements = await _orchestrator.Get(hashedAccountId,
                OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            var unsignedAgreements = agreements.Data.EmployerAgreementsData.TryGetSinglePendingAgreement();

            if (unsignedAgreements == null)
                return RedirectToAction("Index");

            var hashedAgreementId = unsignedAgreements.Pending.HashedAgreementId;

            return RedirectToAction("AboutYourAgreement", new {agreementId = hashedAgreementId});
        }

        [HttpGet]
        [Route("agreements/{agreementId}/about-your-agreement")]
        public async Task<ActionResult> AboutYourAgreement(string agreementId, string hashedAccountId)
        {
            var agreement = await _orchestrator.GetById(agreementId, hashedAccountId,
                OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));
            return View(agreement);

        }

        [HttpGet]
        [Route("agreements/{agreementId}/sign-your-agreement")]
        public ActionResult SignAgreement(GetEmployerAgreementRequest request)
        {
            var paramString = Request?.Url?.Query == null ? string.Empty : $"?{Request.Url.Query}";

            return Redirect(
                Url.LegacyEasAccountAction($"agreements/{request.AgreementId}/sign-your-agreement?{paramString}"));
        }

        [HttpGet]
        [Route("agreements/{agreementId}/next")]
        public ActionResult NextSteps(string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction("agreements/{agreementId}/next"));
        }

        [HttpGet]
        [Route("agreements/{agreementId}/agreement-pdf")]
        public ActionResult GetPdfAgreement(string agreementId, string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction($"agreements/{agreementId}/agreement-pdf"));
        }

        [HttpGet]
        [Route("agreements/{agreementId}/signed-agreement-pdf")]
        public ActionResult GetSignedPdfAgreement(string agreementId, string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction("agreements/{agreementId}/signed-agreement-pdf"));
        }

        [HttpGet]
        [Route("agreements/remove")]
        public ActionResult GetOrganisationsToRemove(string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction("agreements/remove"));
        }

        [HttpGet]
        [Route("agreements/remove/{agreementId}")]
        public ActionResult ConfirmRemoveOrganisation(string agreementId, string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction($"agreements/{agreementId}/next"));
        }

        [HttpPost]
        [Route("agreements/{agreementId}/sign")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Sign(string agreementId, string hashedAccountId)
        {


            var userInfo = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
            var agreement = await _orchestrator.GetById(agreementId, hashedAccountId, userInfo);
            var response = await _orchestrator.SignAgreement(agreementId, hashedAccountId, userInfo, DateTime.UtcNow,
                agreement.Data.EmployerAgreement.LegalEntityName);

            if (response.Status == HttpStatusCode.OK)
            {
                FlashMessageViewModel flashMessage = new FlashMessageViewModel
                {
                    Headline = "Agreement signed",
                    Severity = FlashMessageSeverityLevel.Success
                };

                ActionResult result;

                if (response.Data.HasFurtherPendingAgreements)
                {
                    flashMessage.Message =
                        "You've successfully signed an organisation agreement. There are outstanding agreements to be signed. Review the list below to sign all remaining agreements.";

                    result = RedirectToAction(ControllerConstants.IndexActionName,
                        ControllerConstants.EmployerAgreementControllerName,
                        new {hashedAccountId, agreementSigned = true});
                }
                else
                {
                    flashMessage.Headline = "All agreements signed";
                    flashMessage.Message = "You've successfully signed all of your organisation agreements.";
                    result = RedirectToAction(ControllerConstants.NextStepsActionName);
                }

                AddFlashMessageToCookie(flashMessage);

                return result;
            }


            agreement.Exception = response.Exception;
            agreement.Status = response.Status;

            return View(ControllerConstants.SignAgreementViewName, agreement);
        }


        [HttpPost]
        [Route("agreements/remove/{agreementId}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveOrganisation(string hashedAccountId, string agreementId, ConfirmLegalAgreementToRemoveViewModel model)

        {

            var response = await _orchestrator.RemoveLegalAgreement(model, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            if (response.Status == HttpStatusCode.OK)
            {
                AddFlashMessageToCookie(response.FlashMessage);

                return RedirectToAction(ControllerConstants.IndexActionName, new { hashedAccountId });
            }
            if (response.Status == HttpStatusCode.BadRequest)
            {
                AddFlashMessageToCookie(response.FlashMessage);
                return RedirectToAction(ControllerConstants.ConfirmRemoveOrganisationActionName, new { hashedAccountId, agreementId });
            }

            return RedirectToAction(ControllerConstants.IndexActionName, new { hashedAccountId });
        }
    }
}