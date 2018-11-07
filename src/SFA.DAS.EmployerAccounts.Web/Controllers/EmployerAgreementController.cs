using System;
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

            var model = await _orchestrator.Get(hashedAccountId, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

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
        public ActionResult Details(string agreementId, string hashedAccountId, FlashMessageViewModel flashMessage)
        {
            var paramString = Request?.Url?.Query == null ? string.Empty : $"?{Request.Url.Query}";

            return Redirect(Url.LegacyEasAccountAction($"agreements/{agreementId}/details{paramString}"));
        }

        [HttpGet]
        [Route("agreements/{agreementId}/view")]
        public ActionResult View(string agreementId, string hashedAccountId, FlashMessageViewModel flashMessage)
        {
            var paramString = Request?.Url?.Query == null ? string.Empty : $"?{Request.Url.Query}";

            return Redirect(Url.LegacyEasAccountAction($"agreements/{agreementId}/view?{paramString}"));
        }

        [HttpGet]
        [Route("agreements/unsigned/view")]
        public async Task<ActionResult> ViewUnsignedAgreements(string hashedAccountId)
        {
            var agreements = await _orchestrator.Get(hashedAccountId, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            var unsignedAgreements = agreements.Data.EmployerAgreementsData.TryGetSinglePendingAgreement();

            if (unsignedAgreements == null)
                return RedirectToAction("Index");

            var hashedAgreementId = unsignedAgreements.Pending.HashedAgreementId;

            return RedirectToAction("AboutYourAgreement", new { agreementId = hashedAgreementId });
        }

        [HttpGet]
        [Route("agreements/{agreementId}/about-your-agreement")]
        public ActionResult AboutYourAgreement(string agreementId, string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction($"agreements/{agreementId}/about-your-agreement"));
        }

        [HttpGet]
        [Route("agreements/{agreementId}/sign-your-agreement")]
        public ActionResult SignAgreement(GetEmployerAgreementRequest request)
        {
            var paramString = Request?.Url?.Query == null ? string.Empty : $"?{Request.Url.Query}";

            return Redirect(Url.LegacyEasAccountAction($"agreements/{request.AgreementId}/sign-your-agreement?{paramString}"));
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

    }
}