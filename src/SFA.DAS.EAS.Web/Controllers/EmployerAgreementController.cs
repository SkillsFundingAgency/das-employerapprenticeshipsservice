using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Application.Queries.GetEmployerAgreement;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Helpers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Organisation;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers
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

        [HttpGet]
        [Route("agreements")]
        public async Task<ActionResult> Index()
        {
            return Redirect(Url.EmployerAccountsAction("agreements"));
        }

        [HttpGet]
        [Route("agreements/{agreementId}/details")]
        public async Task<ActionResult> Details(string agreementId)
        {

            return
                Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/details"));
        }

        [HttpGet]
        [Route("agreements/{agreementId}/view")]
        public async Task<ActionResult> View(string agreementId)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/view"));
        }

        [HttpGet]
        [Route("agreements/unsigned/view")]
        public async Task<ActionResult> ViewUnsignedAgreements()
        {
            return Redirect(Url.EmployerAccountsAction("agreements/unsigned/view"));
        }

        [HttpGet]
        [Route("agreements/{agreementId}/about-your-agreement")]
        public async Task<ActionResult> AboutYourAgreement(string agreementid)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/{agreementid}/about-your-agreement"));

        }

        [HttpGet]
        [Route("agreements/{agreementId}/sign-your-agreement")]
        public async Task<ActionResult> SignAgreement(string agreementId)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/sign-your-agreement"));
        }

        [HttpGet]
        [Route("agreements/{agreementId}/next")]
        public async Task<ActionResult> NextSteps(string agreementId)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/next"));
        }


        [HttpGet]
        [Route("agreements/{agreementId}/agreement-pdf")]
        public async Task<ActionResult> GetPdfAgreement(string agreementId)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/agreement-pdf"));

        }

        [HttpGet]
        [Route("agreements/{agreementId}/signed-agreement-pdf")]
        public async Task<ActionResult> GetSignedPdfAgreement(string agreementId)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/signed-agreement-pdf"));
        }

        [HttpGet]
        [Route("agreements/remove")]
        public async Task<ActionResult> GetOrganisationsToRemove(string hashedAccountId)
        {
            var model = await _orchestrator.GetLegalAgreementsToRemove(hashedAccountId, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            return View(model);
        }

        [HttpGet]
        [Route("agreements/remove/{agreementId}")]
        public async Task<ActionResult> ConfirmRemoveOrganisation(string agreementId, string hashedAccountId)
        {
            var model = await _orchestrator.GetConfirmRemoveOrganisationViewModel(agreementId, hashedAccountId, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

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
            return Redirect(Url.EmployerAccountsAction($"agreements/remove/{agreementId}"));
        }

    }
}